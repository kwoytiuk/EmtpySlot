using Microsoft.AspNetCore.Components.WebView.Maui;

namespace EmptySlot.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Add root component for Blazor app
        blazorWebView.RootComponents.Add(new RootComponent
        {
            Selector = "#app",
            ComponentType = typeof(EmptySlot.Shared.UI.App)
        });
    }
}
