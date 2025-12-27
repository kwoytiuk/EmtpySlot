using EmptySlot.Mobile.ViewModels;
using EmptySlot.Shared.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

namespace EmptySlot.Mobile.Pages;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Load providers and add pins
        await _viewModel.LoadProvidersCommand.ExecuteAsync(null);

        // Clear existing pins
        map.Pins.Clear();

        // Add pins for each provider
        foreach (var provider in _viewModel.Providers)
        {
            if (provider.Locations?.Any() == true)
            {
                var location = provider.Locations.First();
                if (location.Latitude.HasValue && location.Longitude.HasValue)
                {
                    var pin = new Pin
                    {
                        Label = provider.BusinessName,
                        Address = $"{location.City} - {provider.RatingAverage:F1}★ ({provider.RatingCount} reviews)",
                        Type = PinType.Place,
                        Location = new Location(location.Latitude.Value, location.Longitude.Value)
                    };

                    // Handle pin click
                    pin.MarkerClicked += async (s, args) =>
                    {
                        args.HideInfoWindow = false;
                        await _viewModel.SelectProviderCommand.ExecuteAsync(provider);
                    };

                    map.Pins.Add(pin);
                }
            }
        }

        // Move to the map region
        if (_viewModel.MapRegion != null)
        {
            map.MoveToRegion(_viewModel.MapRegion);
        }
    }
}
