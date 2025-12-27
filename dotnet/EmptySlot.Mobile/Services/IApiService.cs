using EmptySlot.Shared.Models;

namespace EmptySlot.Mobile.Services;

public interface IApiService
{
    Task<List<Provider>> SearchProvidersAsync(SearchProvidersRequest request);
    Task<Provider?> GetProviderAsync(Guid id);
    Task<List<Appointment>> GetMyAppointmentsAsync();
    Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto);
    Task<List<ServiceCategory>> GetCategoriesAsync();
    Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(Guid providerId, DateTime? startDate = null, DateTime? endDate = null);
    Task BlockTimeSlotAsync(Guid timeSlotId);
    Task UnblockTimeSlotAsync(Guid timeSlotId);
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

public record TimeSlotDto(
    Guid Id,
    DateOnly Date,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string StaffMemberName,
    string DisplayTime);
