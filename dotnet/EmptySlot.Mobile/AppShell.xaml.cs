using EmptySlot.Mobile.Pages;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Services;

namespace EmptySlot.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(BookingPage), typeof(BookingPage));
        Routing.RegisterRoute(nameof(MapPage), typeof(MapPage));

        // Handle navigation after shell is loaded
        Loaded += OnShellLoaded;
    }

    private async void OnShellLoaded(object? sender, EventArgs e)
    {
        var authService = Handler?.MauiContext?.Services.GetService<IAuthService>();

        if (authService != null && !authService.IsAuthenticated)
        {
            await GoToAsync("//LoginPage");
        }
    }
}
