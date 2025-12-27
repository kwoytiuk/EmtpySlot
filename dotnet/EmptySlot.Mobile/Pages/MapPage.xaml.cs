using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class MapPage : ContentPage
{
    private readonly MapViewModel _viewModel;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;

        // Subscribe to collection changes to update map when providers load
        _viewModel.Providers.CollectionChanged += (s, e) =>
        {
            LoadMapHtml();
        };

        // Handle navigation from map popups
        MapWebView.Navigating += OnWebViewNavigating;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProvidersCommand.ExecuteAsync(null);
    }

    private void LoadMapHtml()
    {
        if (_viewModel.Providers.Count > 0)
        {
            var html = _viewModel.GenerateMapHtml();
            MapWebView.Source = new HtmlWebViewSource { Html = html };
        }
    }

    private async void OnWebViewNavigating(object? sender, WebNavigatingEventArgs e)
    {
        // Check if it's our custom URL scheme
        if (e.Url.StartsWith("emptyslot://"))
        {
            e.Cancel = true; // Prevent actual navigation

            try
            {
                var uri = new Uri(e.Url);
                var path = uri.Host;
                var segments = uri.AbsolutePath.TrimStart('/').Split('/');

                System.Diagnostics.Debug.WriteLine($"Map navigation: {e.Url}");

                if (path == "provider" && segments.Length > 0)
                {
                    // Navigate to provider details
                    var providerId = segments[0];
                    await Shell.Current.GoToAsync($"{nameof(BookingPage)}?ProviderId={providerId}");
                }
                else if (path == "book" && segments.Length >= 2)
                {
                    // Quick book with selected time slot
                    var providerId = segments[0];
                    var timeSlot = Uri.UnescapeDataString(segments[1]);

                    var provider = _viewModel.Providers.FirstOrDefault(p => p.Id.ToString() == providerId);
                    if (provider != null)
                    {
                        // Show quick booking confirmation
                        var result = await Shell.Current.DisplayActionSheet(
                            $"Quick Book at {provider.BusinessName}",
                            "Cancel",
                            null,
                            $"Book for {timeSlot}",
                            "View All Services"
                        );

                        if (result == $"Book for {timeSlot}")
                        {
                            // Navigate to booking page with time slot pre-selected
                            await Shell.Current.GoToAsync($"{nameof(BookingPage)}?ProviderId={providerId}");
                            // Note: You could pass the time slot as a query parameter too if BookingPage supports it
                        }
                        else if (result == "View All Services")
                        {
                            await Shell.Current.GoToAsync($"{nameof(BookingPage)}?ProviderId={providerId}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error handling map navigation: {ex.Message}");
            }
        }
    }
}
