using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;

namespace EmptySlot.Mobile.ViewModels;

public partial class ProviderDashboardViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string providerName = string.Empty;

    [ObservableProperty]
    private string businessName = string.Empty;

    [ObservableProperty]
    private Guid providerId;

    public ProviderDashboardViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Provider Dashboard";
    }

    [RelayCommand]
    async Task LoadDashboard()
    {
        var providerIdString = Preferences.Get("ProviderId", string.Empty);
        if (string.IsNullOrEmpty(providerIdString))
        {
            await Shell.Current.DisplayAlert("Error", "Please log in first", "OK");
            await Shell.Current.GoToAsync("///ProviderLoginPage");
            return;
        }

        ProviderId = Guid.Parse(providerIdString);
        ProviderName = Preferences.Get("ProviderName", "");
        BusinessName = Preferences.Get("BusinessName", "");
    }

    [RelayCommand]
    async Task ManageStaff()
    {
        await Shell.Current.GoToAsync($"StaffManagementPage?ProviderId={ProviderId}");
    }

    [RelayCommand]
    async Task ManageServices()
    {
        await Shell.Current.GoToAsync($"ServiceManagementPage?ProviderId={ProviderId}");
    }

    [RelayCommand]
    async Task ViewAppointments()
    {
        await Shell.Current.DisplayAlert("Coming Soon", "Appointments view will be available soon", "OK");
    }

    [RelayCommand]
    async Task Logout()
    {
        Preferences.Remove("ProviderId");
        Preferences.Remove("ProviderName");
        Preferences.Remove("BusinessName");

        await Shell.Current.GoToAsync("///ProviderLoginPage");
    }
}
