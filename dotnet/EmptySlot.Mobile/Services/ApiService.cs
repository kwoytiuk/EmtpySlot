using System.Net.Http.Headers;
using System.Net.Http.Json;
using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private const string BaseUrl = "http://10.0.2.2:5000/api"; // For Android emulator

    public ApiService(IAuthService authService)
    {
        _authService = authService;
        _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    private async Task AddAuthHeaderAsync()
    {
        var token = await _authService.GetTokenAsync();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<List<Provider>> SearchProvidersAsync(SearchProvidersRequest request)
    {
        var queryParams = new List<string>();
        if (request.Latitude.HasValue) queryParams.Add($"latitude={request.Latitude}");
        if (request.Longitude.HasValue) queryParams.Add($"longitude={request.Longitude}");
        if (request.RadiusKm.HasValue) queryParams.Add($"radiusKm={request.RadiusKm}");
        if (request.CategoryId.HasValue) queryParams.Add($"categoryId={request.CategoryId}");
        if (request.MinRating.HasValue) queryParams.Add($"minRating={request.MinRating}");
        if (request.Verified.HasValue) queryParams.Add($"verified={request.Verified}");

        var query = string.Join("&", queryParams);
        var response = await _httpClient.GetAsync($"/providers/search?{query}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Provider>>() ?? new List<Provider>();
    }

    public async Task<Provider?> GetProviderAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/providers/{id}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Provider>();
    }

    public async Task<List<Appointment>> GetMyAppointmentsAsync()
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.GetAsync("/appointments");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Appointment>>() ?? new List<Appointment>();
    }

    public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("/appointments", dto);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Appointment>()
            ?? throw new Exception("Failed to create appointment");
    }

    public async Task<List<ServiceCategory>> GetCategoriesAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/categories");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<ServiceCategory>>() ?? new List<ServiceCategory>();
        }
        catch (HttpRequestException ex)
        {
            System.Diagnostics.Debug.WriteLine($"HTTP Error getting categories: {ex.Message}");
            throw new Exception($"Network error: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error getting categories: {ex.Message}");
            throw;
        }
    }
}
