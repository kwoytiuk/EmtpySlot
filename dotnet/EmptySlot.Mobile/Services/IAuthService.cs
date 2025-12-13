using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.Services;

public interface IAuthService
{
    Task<(bool Success, string? Error)> LoginAsync(string email, string password);
    Task<(bool Success, string? Error)> RegisterAsync(string email, string password, string fullName);
    Task<string?> GetTokenAsync();
    Task<Profile?> GetCurrentUserAsync();
    Task LogoutAsync();
    bool IsAuthenticated { get; }
}
