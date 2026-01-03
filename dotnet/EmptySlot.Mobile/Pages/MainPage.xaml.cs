using Microsoft.AspNetCore.Components.WebView.Maui;

namespace EmptySlot.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        System.Diagnostics.Debug.WriteLine("===== MAINPAGE CONSTRUCTOR START =====");
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("===== MAINPAGE InitializeComponent DONE =====");

        // Add root component programmatically
        blazorWebView.RootComponents.Add(new RootComponent
        {
            Selector = "#app",
            ComponentType = typeof(EmptySlot.Shared.UI.App)
        });

        System.Diagnostics.Debug.WriteLine($"===== ROOT COMPONENT ADDED: {blazorWebView.RootComponents.Count} components =====");
        System.Diagnostics.Debug.WriteLine($"===== Component Type: {typeof(EmptySlot.Shared.UI.App).FullName} =====");
    }
}
