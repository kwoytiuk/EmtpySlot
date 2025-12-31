using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using EmptySlot.Mobile.Services;

namespace EmptySlot.Mobile.ViewModels;

public partial class ProviderRegistrationViewModel : BaseViewModel
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string fullName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private string businessName = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string phone = string.Empty;

    [ObservableProperty]
    private string addressLine1 = string.Empty;

    [ObservableProperty]
    private string city = string.Empty;

    [ObservableProperty]
    private string postalCode = string.Empty;

    public ProviderRegistrationViewModel(IApiService apiService)
    {
        _apiService = apiService;
        Title = "Provider Registration";
    }

    [RelayCommand]
    async Task Register()
    {
        // Validation
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Email) ||
            string.IsNullOrWhiteSpace(Password) || string.IsNullOrWhiteSpace(BusinessName) ||
            string.IsNullOrWhiteSpace(Phone))
        {
            await Shell.Current.DisplayAlert("Error", "Please fill in all required fields", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Shell.Current.DisplayAlert("Error", "Passwords do not match", "OK");
            return;
        }

        if (Password.Length < 6)
        {
            await Shell.Current.DisplayAlert("Error", "Password must be at least 6 characters", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new ProviderRegistrationRequest(
                FullName: FullName,
                Email: Email,
                Password: Password,
                BusinessName: BusinessName,
                Description: Description,
                Phone: Phone,
                AddressLine1: AddressLine1,
                AddressLine2: null,
                City: City,
                StateProvince: "AB",
                PostalCode: PostalCode,
                Latitude: null,
                Longitude: null);

            var response = await _apiService.ProviderRegisterAsync(request);

            await Shell.Current.DisplayAlert("Success", "Registration successful! Please log in.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Registration failed: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task GoToLogin()
    {
        await Shell.Current.GoToAsync("..");
    }
}
