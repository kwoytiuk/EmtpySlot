using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Mobile.Pages;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.ViewModels;

[QueryProperty(nameof(CategoryId), "CategoryId")]
[QueryProperty(nameof(City), "City")]
[QueryProperty(nameof(Latitude), "Latitude")]
[QueryProperty(nameof(Longitude), "Longitude")]
[QueryProperty(nameof(Radius), "Radius")]
[QueryProperty(nameof(VerifiedOnly), "VerifiedOnly")]
[QueryProperty(nameof(FeaturedOnly), "FeaturedOnly")]
public partial class MapViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Provider> providers = new();

    [ObservableProperty]
    private string searchCity = "Calgary";

    // Query parameters from search
    [ObservableProperty]
    private string categoryId = "";

    [ObservableProperty]
    private string city = "Calgary";

    [ObservableProperty]
    private string latitude = "";

    [ObservableProperty]
    private string longitude = "";

    [ObservableProperty]
    private string radius = "50";

    [ObservableProperty]
    private string verifiedOnly = "false";

    [ObservableProperty]
    private string featuredOnly = "false";

    [ObservableProperty]
    private string mapHtmlSource = "";

    public bool HasProviders => Providers.Count > 0;
    public bool ShowEmptyState => !IsBusy && Providers.Count == 0;

    public MapViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Map View";
    }

    partial void OnCityChanged(string value)
    {
        SearchCity = value;
    }

    [RelayCommand]
    async Task LoadProviders()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            List<Provider> results;

            // First, check if we have cached results from the search page
            if (SearchViewModel.LastSearchResults != null && SearchViewModel.LastSearchResults.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"MapViewModel - Using cached search results ({SearchViewModel.LastSearchResults.Count} providers)");
                results = SearchViewModel.LastSearchResults;
            }
            else
            {
                // No cached results, perform fresh search
                System.Diagnostics.Debug.WriteLine("MapViewModel - No cached results, performing fresh search");

                // Parse query parameters
                Guid? catId = string.IsNullOrEmpty(CategoryId) ? (Guid?)null : Guid.Parse(CategoryId);
                double? lat = string.IsNullOrEmpty(Latitude) ? null : double.Parse(Latitude);
                double? lon = string.IsNullOrEmpty(Longitude) ? null : double.Parse(Longitude);
                int radiusKm = string.IsNullOrEmpty(Radius) ? 50 : int.Parse(Radius);
                bool? verified = VerifiedOnly == "true" ? true : null;

                System.Diagnostics.Debug.WriteLine($"MapViewModel - Search filters:");
                System.Diagnostics.Debug.WriteLine($"  CategoryId: {catId?.ToString() ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"  Location: {lat?.ToString() ?? "null"}, {lon?.ToString() ?? "null"}");
                System.Diagnostics.Debug.WriteLine($"  Radius: {radiusKm} km");
                System.Diagnostics.Debug.WriteLine($"  Verified: {verified?.ToString() ?? "null"}");

                // Search for providers using the filters
                var request = new SearchProvidersRequest(
                    CategoryId: catId,
                    Latitude: lat,
                    Longitude: lon,
                    RadiusKm: radiusKm,
                    MinRating: null,
                    Verified: verified
                );

                results = await _apiService.SearchProvidersAsync(request);
                System.Diagnostics.Debug.WriteLine($"MapViewModel - API returned {results.Count} providers");
            }

            Providers.Clear();
            int addedCount = 0;
            int skippedNoLocation = 0;
            int skippedInvalidCoords = 0;

            foreach (var provider in results)
            {
                // Only add providers that have location data
                if (provider.Locations?.Any() == true)
                {
                    var location = provider.Locations.First();
                    if (location.Latitude != 0.0 && location.Longitude != 0.0)
                    {
                        Providers.Add(provider);
                        addedCount++;
                    }
                    else
                    {
                        skippedInvalidCoords++;
                        System.Diagnostics.Debug.WriteLine($"  Skipped {provider.BusinessName}: invalid coords ({location.Latitude}, {location.Longitude})");
                    }
                }
                else
                {
                    skippedNoLocation++;
                    System.Diagnostics.Debug.WriteLine($"  Skipped {provider.BusinessName}: no location data");
                }
            }

            System.Diagnostics.Debug.WriteLine($"MapViewModel - Final: {addedCount} added, {skippedNoLocation} no location, {skippedInvalidCoords} invalid coords");

            // Notify property changes for visibility bindings
            OnPropertyChanged(nameof(HasProviders));
            OnPropertyChanged(nameof(ShowEmptyState));

            if (Providers.Count == 0 && results.Count > 0)
            {
                await Shell.Current.DisplayAlert("No Locations",
                    $"Found {results.Count} providers but none have valid location data to display on the map.",
                    "OK");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"MapViewModel - Error: {ex}");
            await Shell.Current.DisplayAlert("Error", $"Failed to load providers: {ex.Message}\n\nCheck debug output for details.", "OK");
        }
        finally
        {
            IsBusy = false;
            OnPropertyChanged(nameof(ShowEmptyState));
        }
    }

    public string GenerateMapHtml()
    {
        if (Providers.Count == 0)
            return "";

        // Calculate center point (average of all provider locations)
        var avgLat = Providers.Average(p => p.Locations.First().Latitude);
        var avgLng = Providers.Average(p => p.Locations.First().Longitude);

        // Generate markers JavaScript
        var markers = string.Join("\n", Providers.Select(p =>
        {
            var loc = p.Locations.First();
            var name = p.BusinessName.Replace("'", "\\'");
            var address = $"{loc.AddressLine1}, {loc.City}".Replace("'", "\\'");
            var rating = p.RatingAverage.ToString("F1");

            return $@"
                L.marker([{loc.Latitude}, {loc.Longitude}], {{
                    icon: L.divIcon({{
                        className: 'custom-pin',
                        html: '<div style=""background: linear-gradient(135deg, #EC4899 0%, #A855F7 100%); width: 32px; height: 32px; border-radius: 50% 50% 50% 0; transform: rotate(-45deg); border: 3px solid white; box-shadow: 0 2px 8px rgba(0,0,0,0.3); display: flex; align-items: center; justify-content: center;""><span style=""transform: rotate(45deg); font-size: 18px;"">📍</span></div>',
                        iconSize: [32, 32],
                        iconAnchor: [16, 32]
                    }})
                }})
                .bindPopup(`
                    <div style=""font-family: system-ui, sans-serif; min-width: 200px;"">
                        <h3 style=""margin: 0 0 8px 0; font-size: 16px; color: #1F2937;"">{name}</h3>
                        <div style=""background: #FEF3C7; padding: 4px 8px; border-radius: 6px; display: inline-block; margin-bottom: 8px;"">
                            <span style=""color: #D97706; font-weight: bold;"">⭐ {rating}</span>
                        </div>
                        <p style=""margin: 0; font-size: 13px; color: #6B7280;"">{address}</p>
                    </div>
                `)
                .addTo(map);";
        }));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <link rel=""stylesheet"" href=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.css"" />
    <script src=""https://unpkg.com/leaflet@1.9.4/dist/leaflet.js""></script>
    <style>
        body {{ margin: 0; padding: 0; }}
        #map {{ width: 100%; height: 100vh; }}
    </style>
</head>
<body>
    <div id=""map""></div>
    <script>
        var map = L.map('map').setView([{avgLat}, {avgLng}], 12);

        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
            attribution: '© OpenStreetMap contributors',
            maxZoom: 19
        }}).addTo(map);

        {markers}

        // Fit bounds to show all markers
        var group = L.featureGroup();
        map.eachLayer(function(layer) {{
            if (layer instanceof L.Marker) {{
                group.addLayer(layer);
            }}
        }});
        if (group.getLayers().length > 0) {{
            map.fitBounds(group.getBounds().pad(0.1));
        }}
    </script>
</body>
</html>";
    }

    [RelayCommand]
    async Task SelectProvider(Provider provider)
    {
        if (provider == null) return;

        var result = await Shell.Current.DisplayActionSheet(
            provider.BusinessName,
            "Cancel",
            null,
            "View Details",
            "Book Appointment"
        );

        if (result == "View Details" || result == "Book Appointment")
        {
            await Shell.Current.GoToAsync($"{nameof(BookingPage)}?ProviderId={provider.Id}");
        }
    }
}
