using Microsoft.Maui.Controls;

namespace EmptySlot.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
}