using EmptySlot.Mobile.Services;

namespace EmptySlot.Mobile;

public partial class App : Application
{
    private readonly IAuthService _authService;

    public App(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        MainPage = new AppShell();
    }
}
