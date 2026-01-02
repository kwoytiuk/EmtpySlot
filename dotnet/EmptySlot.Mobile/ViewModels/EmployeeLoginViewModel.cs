using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;
using EmptySlot.Shared.Services;

namespace EmptySlot.Mobile.ViewModels;

public partial class EmployeeLoginViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    public EmployeeLoginViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Employee Login";
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

            var response = await _apiService.EmployeeLoginAsync(Email, Password);

            // Store employee info in preferences
            Preferences.Set("StaffId", response.StaffId.ToString());
            Preferences.Set("StaffName", response.Name);
            Preferences.Set("StaffProviderId", response.ProviderId.ToString());
            Preferences.Set("StaffBusinessName", response.BusinessName);

            await Shell.Current.DisplayAlert("Success", $"Welcome, {response.Name}!", "OK");
            await Shell.Current.GoToAsync($"///EmployeeDashboardPage");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Login failed: {ex.Message}\n\nHint: Password is your name without spaces, lowercase", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
