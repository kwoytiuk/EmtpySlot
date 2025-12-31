using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class StaffManagementPage : ContentPage
{
    private readonly StaffManagementViewModel _viewModel;

    public StaffManagementPage(StaffManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadStaffCommand.ExecuteAsync(null);
    }
}
