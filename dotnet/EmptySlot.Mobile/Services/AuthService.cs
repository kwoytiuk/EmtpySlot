using System.Net.Http.Json;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://localhost:7001/api"; // Change to your API URL
    private string? _token;
    private Profile? _currentUser;

    public bool IsAuthenticated => !string.IsNullOrEmpty(_token);

    public AuthService()
    {
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    public async Task<(bool Success, string? Error)> LoginAsync(string email, string password)
    {
        try
        {
            var request = new { Email = email, Password = password };
            var response = await _httpClient.PostAsJsonAsync("/auth/login", request);

            if (!response.IsSuccessStatusCode)
            {
                return (false, "Invalid email or password");
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result != null)
            {
                _token = result.Token;
                _currentUser = result.Profile;
                await SecureStorage.SetAsync("auth_token", _token);
                return (true, null);
            }

            return (false, "Failed to parse response");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string fullName)
    {
        try
        {
            var request = new { Email = email, Password = password, FullName = fullName };
            var response = await _httpClient.PostAsJsonAsync("/auth/register", request);

            if (!response.IsSuccessStatusCode)
            {
                return (false, "Registration failed");
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (result != null)
            {
                _token = result.Token;
                _currentUser = result.Profile;
                await SecureStorage.SetAsync("auth_token", _token);
                return (true, null);
            }

            return (false, "Failed to parse response");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public async Task<string?> GetTokenAsync()
    {
        if (string.IsNullOrEmpty(_token))
        {
            _token = await SecureStorage.GetAsync("auth_token");
        }
        return _token;
    }

    public Task<Profile?> GetCurrentUserAsync()
    {
        return Task.FromResult(_currentUser);
    }

    public async Task LogoutAsync()
    {
        _token = null;
        _currentUser = null;
        SecureStorage.Remove("auth_token");
        await Task.CompletedTask;
    }

    private record LoginResponse(string Token, Profile Profile);
}
