using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class BookingPage : ContentPage
{
    private readonly BookingViewModel _viewModel;

    public BookingPage(BookingViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadProviderCommand.ExecuteAsync(null);
    }

    private void OnDateSelected(object sender, DateChangedEventArgs e)
    {
        _viewModel.OnDateChanged();
    }
}
