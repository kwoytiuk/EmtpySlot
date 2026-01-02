using EmptySlot.Mobile.Pages;

namespace EmptySlot.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // Blazor handles all routing now - no need for Shell routes
    }
}
