using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
