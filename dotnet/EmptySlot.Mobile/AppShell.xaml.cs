using EmptySlot.Mobile.Pages;

namespace EmptySlot.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        System.Diagnostics.Debug.WriteLine("===== APPSHELL CONSTRUCTOR START =====");
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("===== APPSHELL InitializeComponent DONE =====");

        // Blazor handles all routing now - no need for Shell routes
    }
}
