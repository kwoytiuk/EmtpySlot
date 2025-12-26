using Microsoft.Maui.Controls;

namespace EmptySlot.Mobile;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        // This replaces setting MainPage in the constructor
        return new Window(new AppShell());
    }
}