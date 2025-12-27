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

            // Parse query parameters
            Guid? catId = string.IsNullOrEmpty(CategoryId) ? (Guid?)null : Guid.Parse(CategoryId);
            double? lat = string.IsNullOrEmpty(Latitude) ? null : double.Parse(Latitude);
            double? lon = string.IsNullOrEmpty(Longitude) ? null : double.Parse(Longitude);
            int radiusKm = string.IsNullOrEmpty(Radius) ? 50 : int.Parse(Radius);
            bool? verified = VerifiedOnly == "true" ? true : null;

            System.Diagnostics.Debug.WriteLine($"MapViewModel - Loading providers with filters:");
            System.Diagnostics.Debug.WriteLine($"  CategoryId: {catId?.ToString() ?? "null"}");
            System.Diagnostics.Debug.WriteLine($"  Location: {lat?.ToString() ?? "null"}, {lon?.ToString() ?? "null"}");
            System.Diagnostics.Debug.WriteLine($"  Radius: {radiusKm} km");
            System.Diagnostics.Debug.WriteLine($"  Verified: {verified?.ToString() ?? "null"}");

            // Search for providers using the same filters from search page
            var request = new SearchProvidersRequest(
                CategoryId: catId,
                Latitude: lat,
                Longitude: lon,
                RadiusKm: radiusKm,
                MinRating: null,
                Verified: verified
            );

            var results = await _apiService.SearchProvidersAsync(request);
            System.Diagnostics.Debug.WriteLine($"MapViewModel - API returned {results.Count} providers");

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
                    if (location.Latitude > 0.0 && location.Longitude > 0.0)
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
        }
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
