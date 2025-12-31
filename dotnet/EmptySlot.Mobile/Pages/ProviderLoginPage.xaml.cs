using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class ProviderLoginPage : ContentPage
{
    public ProviderLoginPage(ProviderLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnBackToCustomer(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("///BrowsePage");
    }
}
