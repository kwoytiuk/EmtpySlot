using Microsoft.AspNetCore.Components.WebView.Maui;

namespace EmptySlot.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        System.Diagnostics.Debug.WriteLine("===== MAINPAGE CONSTRUCTOR START =====");

        try
        {
            InitializeComponent();
            System.Diagnostics.Debug.WriteLine("===== MAINPAGE InitializeComponent DONE =====");

            // Add root component for Blazor app
            System.Diagnostics.Debug.WriteLine($"===== ADDING ROOT COMPONENT: {typeof(EmptySlot.Shared.UI.App).FullName} =====");

            blazorWebView.RootComponents.Add(new RootComponent
            {
                Selector = "#app",
                ComponentType = typeof(EmptySlot.Shared.UI.App)
            });

            System.Diagnostics.Debug.WriteLine("===== ROOT COMPONENT ADDED SUCCESSFULLY =====");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"===== MAINPAGE ERROR: {ex.Message} =====");
            System.Diagnostics.Debug.WriteLine($"===== STACK TRACE: {ex.StackTrace} =====");
        }

        System.Diagnostics.Debug.WriteLine("===== MAINPAGE CONSTRUCTOR END =====");
    }
}
