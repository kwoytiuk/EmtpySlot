namespace EmptySlot.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        System.Diagnostics.Debug.WriteLine("===== MAINPAGE CONSTRUCTOR START =====");
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("===== MAINPAGE INITIALIZED =====");

        // Verify root components were added
        var webView = (Microsoft.AspNetCore.Components.WebView.Maui.BlazorWebView)Content;
        System.Diagnostics.Debug.WriteLine($"===== BlazorWebView has {webView.RootComponents.Count} root components =====");

        foreach (var component in webView.RootComponents)
        {
            System.Diagnostics.Debug.WriteLine($"===== Component: Selector={component.Selector}, Type={component.ComponentType?.FullName} =====");
        }
    }
}
