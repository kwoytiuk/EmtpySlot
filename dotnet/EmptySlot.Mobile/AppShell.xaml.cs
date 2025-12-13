using EmptySlot.Mobile.Pages;

namespace EmptySlot.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(BookingPage), typeof(BookingPage));
    }
}
