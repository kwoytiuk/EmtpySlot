namespace EmptySlot.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        // Add root component for Blazor app
        blazorWebView.RootComponents.Add(new Microsoft.AspNetCore.Components.WebView.RootComponent
        {
            Selector = "#app",
            ComponentType = typeof(EmptySlot.Shared.UI.App)
        });
    }
}
