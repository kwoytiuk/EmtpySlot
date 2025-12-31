using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class ScheduleManagementPage : ContentPage
{
    private readonly ScheduleManagementViewModel _viewModel;

    public ScheduleManagementPage(ScheduleManagementViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadSchedulesCommand.ExecuteAsync(null);
    }
}
