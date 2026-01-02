using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Services;

namespace EmptySlot.Mobile.ViewModels;

public partial class ProviderLoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public ProviderLoginViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Provider Login";
    }

    [RelayCommand]
    async Task Login()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Error", "Please enter email and password", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var response = await _apiService.ProviderLoginAsync(Email, Password);

            // Store provider info in preferences
            Preferences.Set("ProviderId", response.ProviderId.ToString());
            Preferences.Set("ProviderName", response.FullName);
            Preferences.Set("BusinessName", response.BusinessName);

            await Shell.Current.DisplayAlert("Success", $"Welcome back, {response.FullName}!", "OK");
            await Shell.Current.GoToAsync($"///ProviderDashboardPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Login failed: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task GoToRegistration()
    {
        await Shell.Current.GoToAsync("ProviderRegistrationPage");
    }
}
