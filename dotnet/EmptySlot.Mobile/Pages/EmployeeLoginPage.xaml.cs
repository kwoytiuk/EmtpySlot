using EmptySlot.Mobile.ViewModels;

namespace EmptySlot.Mobile.Pages;

public partial class EmployeeLoginPage : ContentPage
{
    public EmployeeLoginPage(EmployeeLoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
