using System.Net.Http.Headers;
using System.Net.Http.Json;
using EmptySlot.Shared.Models;
using EmptySlot.Shared.Services;

namespace EmptySlot.Mobile.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private readonly IAuthService _authService;
    private readonly ICacheService _cache;
    private const string BaseUrl = "http://10.0.2.2:5000/api/"; // For Android emulator - trailing slash is important!

    public ApiService(IAuthService authService, ICacheService cache)
    {
        _authService = authService;
        _cache = cache;
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

        // Check cache first - cache search results for 60 seconds
        var cacheKey = $"search:{query}";
        var cached = _cache.Get<List<Provider>>(cacheKey);
        if (cached != null)
        {
            System.Diagnostics.Debug.WriteLine($"Returning {cached.Count} providers from cache");
            return cached;
        }

        System.Diagnostics.Debug.WriteLine($"Fetching providers from API: {query}");
        var response = await _httpClient.GetAsync($"providers/search?{query}");
        response.EnsureSuccessStatusCode();

        var providers = await response.Content.ReadFromJsonAsync<List<Provider>>() ?? new List<Provider>();

        // Cache for 60 seconds to balance freshness with performance
        _cache.Set(cacheKey, providers, TimeSpan.FromSeconds(60));
        System.Diagnostics.Debug.WriteLine($"Cached {providers.Count} providers");

        return providers;
    }

    public async Task<Provider?> GetProviderAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"providers/{id}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Provider>();
    }

    public async Task<List<Appointment>> GetMyAppointmentsAsync()
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.GetAsync("appointments");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Appointment>>() ?? new List<Appointment>();
    }

    public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsJsonAsync("appointments", dto);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Appointment>()
            ?? throw new Exception("Failed to create appointment");
    }

    public async Task<List<ServiceCategory>> GetCategoriesAsync()
    {
        try
        {
            // Check cache first - categories rarely change
            var cacheKey = "categories";
            var cached = _cache.Get<List<ServiceCategory>>(cacheKey);
            if (cached != null)
            {
                System.Diagnostics.Debug.WriteLine("Returning categories from cache");
                return cached;
            }

            System.Diagnostics.Debug.WriteLine("Fetching categories from API");
            var response = await _httpClient.GetAsync("categories");
            response.EnsureSuccessStatusCode();

            var categories = await response.Content.ReadFromJsonAsync<List<ServiceCategory>>() ?? new List<ServiceCategory>();

            // Cache for 10 minutes
            _cache.Set(cacheKey, categories, TimeSpan.FromMinutes(10));

            return categories;
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

    public async Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(Guid providerId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

        var response = await _httpClient.GetAsync($"timeslots/provider/{providerId}/available{query}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<TimeSlotDto>>() ?? new List<TimeSlotDto>();
    }

    public async Task BlockTimeSlotAsync(Guid timeSlotId)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsync($"timeslots/{timeSlotId}/block", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnblockTimeSlotAsync(Guid timeSlotId)
    {
        await AddAuthHeaderAsync();
        var response = await _httpClient.PostAsync($"timeslots/{timeSlotId}/unblock", null);
        response.EnsureSuccessStatusCode();
    }

    // Provider Authentication
    public async Task<ProviderAuthResponse> ProviderRegisterAsync(ProviderRegistrationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("providerauth/register", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<dynamic>();
        return new ProviderAuthResponse(
            result.profileId,
            result.providerId,
            request.FullName,
            request.Email,
            request.BusinessName,
            null);
    }

    public async Task<ProviderAuthResponse> ProviderLoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("providerauth/login", new { Email = email, Password = password });
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<ProviderAuthResponse>()
            ?? throw new Exception("Failed to parse login response");
    }

    // Employee Authentication
    public async Task<EmployeeAuthResponse> EmployeeLoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("employeeauth/login", new { Email = email, Password = password });
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<EmployeeAuthResponse>()
            ?? throw new Exception("Failed to parse login response");
    }

    // Staff Management
    public async Task<List<StaffMember>> GetProviderStaffAsync(Guid providerId)
    {
        var response = await _httpClient.GetAsync($"staff/provider/{providerId}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<StaffMember>>() ?? new List<StaffMember>();
    }

    public async Task<StaffMember> CreateStaffMemberAsync(CreateStaffRequest request)
    {
        var staff = new StaffMember
        {
            ProviderId = request.ProviderId,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Bio = request.Bio
        };

        var response = await _httpClient.PostAsJsonAsync("staff", staff);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<StaffMember>()
            ?? throw new Exception("Failed to create staff member");
    }

    public async Task<StaffMember> UpdateStaffMemberAsync(Guid staffId, UpdateStaffRequest request)
    {
        var staff = new
        {
            Id = staffId,
            request.Name,
            request.Email,
            request.Phone,
            request.Bio,
            request.IsActive
        };

        var response = await _httpClient.PutAsJsonAsync($"staff/{staffId}", staff);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<StaffMember>()
            ?? throw new Exception("Failed to update staff member");
    }

    public async Task DeleteStaffMemberAsync(Guid staffId)
    {
        var response = await _httpClient.DeleteAsync($"staff/{staffId}");
        response.EnsureSuccessStatusCode();
    }

    // Schedule Management
    public async Task<List<StaffSchedule>> GetStaffSchedulesAsync(Guid staffId)
    {
        var response = await _httpClient.GetAsync($"staff/{staffId}/schedules");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<StaffSchedule>>() ?? new List<StaffSchedule>();
    }

    public async Task<StaffSchedule> CreateScheduleAsync(Guid staffId, CreateScheduleRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"staff/{staffId}/schedules", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<StaffSchedule>()
            ?? throw new Exception("Failed to create schedule");
    }

    public async Task<StaffSchedule> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleRequest request)
    {
        var schedule = new
        {
            Id = scheduleId,
            request.DayOfWeek,
            request.StartTime,
            request.EndTime,
            request.IsActive
        };

        var response = await _httpClient.PutAsJsonAsync($"staff/schedules/{scheduleId}", schedule);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<StaffSchedule>()
            ?? throw new Exception("Failed to update schedule");
    }

    public async Task DeleteScheduleAsync(Guid scheduleId)
    {
        var response = await _httpClient.DeleteAsync($"staff/schedules/{scheduleId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task GenerateTimeSlotsAsync(Guid staffId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

        var query = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

        var response = await _httpClient.PostAsync($"staff/{staffId}/generate-slots{query}", null);
        response.EnsureSuccessStatusCode();
    }

    // Service Management
    public async Task<List<Service>> GetProviderServicesAsync(Guid providerId)
    {
        var response = await _httpClient.GetAsync($"services/provider/{providerId}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Service>>() ?? new List<Service>();
    }

    public async Task<Service> CreateServiceAsync(CreateServiceRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("services", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Service>()
            ?? throw new Exception("Failed to create service");
    }

    public async Task<Service> UpdateServiceAsync(Guid serviceId, UpdateServiceRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"services/{serviceId}", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Service>()
            ?? throw new Exception("Failed to update service");
    }

    public async Task DeleteServiceAsync(Guid serviceId)
    {
        var response = await _httpClient.DeleteAsync($"services/{serviceId}");
        response.EnsureSuccessStatusCode();
    }
}
