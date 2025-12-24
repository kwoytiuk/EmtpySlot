using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.Services;

public interface IApiService
{
    Task<List<Provider>> SearchProvidersAsync(SearchProvidersRequest request);
    Task<Provider?> GetProviderAsync(Guid id);
    Task<List<Appointment>> GetMyAppointmentsAsync();
    Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto);
    Task<List<ServiceCategory>> GetCategoriesAsync();
}

public record SearchProvidersRequest(
    double? Latitude = null,
    double? Longitude = null,
    double? RadiusKm = null,
    Guid? CategoryId = null,
    decimal? MinRating = null,
    bool? Verified = null);

public record CreateAppointmentDto(
    Guid ProviderId,
    Guid LocationId,
    Guid ServiceId,
    Guid? StaffId,
    DateTime AppointmentDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    decimal Price,
    string? CustomerNotes);
