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
}
