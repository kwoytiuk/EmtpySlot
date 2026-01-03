namespace EmptySlot.Mobile.Pages;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        System.Diagnostics.Debug.WriteLine("===== MAINPAGE CONSTRUCTOR START =====");
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("===== MAINPAGE INITIALIZED - Blazor configured in XAML =====");
    }
}
