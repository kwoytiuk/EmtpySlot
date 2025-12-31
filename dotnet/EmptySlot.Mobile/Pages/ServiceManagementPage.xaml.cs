using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class ServiceManagementPage : ContentPage
{
    private readonly ServiceManagementViewModel _viewModel;

    public ServiceManagementPage(ServiceManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadServicesCommand.ExecuteAsync(null);
    }
}
