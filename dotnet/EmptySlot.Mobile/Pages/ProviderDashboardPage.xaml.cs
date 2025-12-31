using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class ProviderDashboardPage : ContentPage
{
    private readonly ProviderDashboardViewModel _viewModel;

    public ProviderDashboardPage(ProviderDashboardViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDashboardCommand.ExecuteAsync(null);
    }
}
