using System.Net.Http.Json;
using EmptySlot.Shared.Models;
using EmptySlot.Shared.Services;

namespace EmptySlot.Web.Services;

public class WebApiService : IApiService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5000/api/";

    public WebApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
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
        var url = $"providers/search?{query}";

        var result = await _httpClient.GetFromJsonAsync<List<Provider>>(url);
        return result ?? new List<Provider>();
    }

    public async Task<Provider?> GetProviderAsync(Guid id)
    {
        return await _httpClient.GetFromJsonAsync<Provider>($"providers/{id}");
    }

    public async Task<List<Appointment>> GetMyAppointmentsAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<Appointment>>("appointments/my");
        return result ?? new List<Appointment>();
    }

    public async Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto)
    {
        var response = await _httpClient.PostAsJsonAsync("appointments", dto);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Appointment>())!;
    }

    public async Task<List<ServiceCategory>> GetCategoriesAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<ServiceCategory>>("categories");
        return result ?? new List<ServiceCategory>();
    }

    public async Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(Guid providerId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

        var query = string.Join("&", queryParams);
        var url = $"timeslots/provider/{providerId}/available?{query}";

        var result = await _httpClient.GetFromJsonAsync<List<TimeSlotDto>>(url);
        return result ?? new List<TimeSlotDto>();
    }

    public Task BlockTimeSlotAsync(Guid timeSlotId)
    {
        return _httpClient.PostAsync($"timeslots/{timeSlotId}/block", null);
    }

    public Task UnblockTimeSlotAsync(Guid timeSlotId)
    {
        return _httpClient.PostAsync($"timeslots/{timeSlotId}/unblock", null);
    }

    public async Task<ProviderAuthResponse> ProviderRegisterAsync(ProviderRegistrationRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/provider/register", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProviderAuthResponse>())!;
    }

    public async Task<ProviderAuthResponse> ProviderLoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/provider/login", new { email, password });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<ProviderAuthResponse>())!;
    }

    public async Task<EmployeeAuthResponse> EmployeeLoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("auth/employee/login", new { email, password });
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<EmployeeAuthResponse>())!;
    }

    public async Task<List<StaffMember>> GetProviderStaffAsync(Guid providerId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<StaffMember>>($"providers/{providerId}/staff");
        return result ?? new List<StaffMember>();
    }

    public async Task<StaffMember> CreateStaffMemberAsync(CreateStaffRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("staff", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StaffMember>())!;
    }

    public async Task<StaffMember> UpdateStaffMemberAsync(Guid staffId, UpdateStaffRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"staff/{staffId}", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StaffMember>())!;
    }

    public Task DeleteStaffMemberAsync(Guid staffId)
    {
        return _httpClient.DeleteAsync($"staff/{staffId}");
    }

    public async Task<List<StaffSchedule>> GetStaffSchedulesAsync(Guid staffId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<StaffSchedule>>($"staff/{staffId}/schedules");
        return result ?? new List<StaffSchedule>();
    }

    public async Task<StaffSchedule> CreateScheduleAsync(Guid staffId, CreateScheduleRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync($"staff/{staffId}/schedules", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StaffSchedule>())!;
    }

    public async Task<StaffSchedule> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"schedules/{scheduleId}", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<StaffSchedule>())!;
    }

    public Task DeleteScheduleAsync(Guid scheduleId)
    {
        return _httpClient.DeleteAsync($"schedules/{scheduleId}");
    }

    public Task GenerateTimeSlotsAsync(Guid staffId, DateTime? startDate = null, DateTime? endDate = null)
    {
        var queryParams = new List<string>();
        if (startDate.HasValue) queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        if (endDate.HasValue) queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");

        var query = string.Join("&", queryParams);
        return _httpClient.PostAsync($"staff/{staffId}/generate-timeslots?{query}", null);
    }

    public async Task<List<Service>> GetProviderServicesAsync(Guid providerId)
    {
        var result = await _httpClient.GetFromJsonAsync<List<Service>>($"providers/{providerId}/services");
        return result ?? new List<Service>();
    }

    public async Task<Service> CreateServiceAsync(CreateServiceRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("services", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Service>())!;
    }

    public async Task<Service> UpdateServiceAsync(Guid serviceId, UpdateServiceRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"services/{serviceId}", request);
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<Service>())!;
    }

    public Task DeleteServiceAsync(Guid serviceId)
    {
        return _httpClient.DeleteAsync($"services/{serviceId}");
    }
}
