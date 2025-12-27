using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Mobile.Pages;
using EmptySlot.Shared.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace EmptySlot.Mobile.ViewModels;

public partial class MapViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private ObservableCollection<Provider> providers = new();

    [ObservableProperty]
    private string searchCity = "Calgary";

    [ObservableProperty]
    private MapSpan? mapRegion;

    public MapViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Map View";

        // Default to Calgary
        MapRegion = MapSpan.FromCenterAndRadius(
            new Location(51.0447, -114.0719), // Calgary coordinates
            Distance.FromKilometers(20));
    }

    [RelayCommand]
    async Task LoadProviders()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;

            // Search for providers in the current city
            var request = new SearchProvidersRequest(
                CategoryId: null,
                Latitude: null,
                Longitude: null,
                RadiusKm: 50,
                MinRating: null,
                Verified: null
            );

            var results = await _apiService.SearchProvidersAsync(request);

            Providers.Clear();
            foreach (var provider in results)
            {
                // Only add providers that have location data
                if (provider.Locations?.Any() == true)
                {
                    var location = provider.Locations.First();
                    if (location.Latitude.HasValue && location.Longitude.HasValue)
                    {
                        Providers.Add(provider);
                    }
                }
            }

            // Center map on providers if we have any
            if (Providers.Any())
            {
                var firstProvider = Providers.First();
                var firstLocation = firstProvider.Locations.First();
                if (firstLocation.Latitude.HasValue && firstLocation.Longitude.HasValue)
                {
                    MapRegion = MapSpan.FromCenterAndRadius(
                        new Location(firstLocation.Latitude.Value, firstLocation.Longitude.Value),
                        Distance.FromKilometers(10));
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Failed to load providers: {ex.Message}", "OK");
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
