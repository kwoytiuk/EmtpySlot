using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class ProviderRegistrationPage : ContentPage
{
    public ProviderRegistrationPage(ProviderRegistrationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
