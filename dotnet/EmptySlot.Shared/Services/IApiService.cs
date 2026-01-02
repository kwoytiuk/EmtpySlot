using EmptySlot.Shared.Models;

namespace EmptySlot.Shared.Services;

public interface IApiService
{
    // Customer APIs
    Task<List<Provider>> SearchProvidersAsync(SearchProvidersRequest request);
    Task<Provider?> GetProviderAsync(Guid id);
    Task<List<Appointment>> GetMyAppointmentsAsync();
    Task<Appointment> CreateAppointmentAsync(CreateAppointmentDto dto);
    Task<List<ServiceCategory>> GetCategoriesAsync();
    Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(Guid providerId, DateTime? startDate = null, DateTime? endDate = null);
    Task BlockTimeSlotAsync(Guid timeSlotId);
    Task UnblockTimeSlotAsync(Guid timeSlotId);

    // Provider APIs
    Task<ProviderAuthResponse> ProviderRegisterAsync(ProviderRegistrationRequest request);
    Task<ProviderAuthResponse> ProviderLoginAsync(string email, string password);

    // Employee APIs
    Task<EmployeeAuthResponse> EmployeeLoginAsync(string email, string password);

    // Staff Management APIs
    Task<List<StaffMember>> GetProviderStaffAsync(Guid providerId);
    Task<StaffMember> CreateStaffMemberAsync(CreateStaffRequest request);
    Task<StaffMember> UpdateStaffMemberAsync(Guid staffId, UpdateStaffRequest request);
    Task DeleteStaffMemberAsync(Guid staffId);

    // Schedule Management APIs
    Task<List<StaffSchedule>> GetStaffSchedulesAsync(Guid staffId);
    Task<StaffSchedule> CreateScheduleAsync(Guid staffId, CreateScheduleRequest request);
    Task<StaffSchedule> UpdateScheduleAsync(Guid scheduleId, UpdateScheduleRequest request);
    Task DeleteScheduleAsync(Guid scheduleId);
    Task GenerateTimeSlotsAsync(Guid staffId, DateTime? startDate = null, DateTime? endDate = null);

    // Service Management APIs
    Task<List<Service>> GetProviderServicesAsync(Guid providerId);
    Task<Service> CreateServiceAsync(CreateServiceRequest request);
    Task<Service> UpdateServiceAsync(Guid serviceId, UpdateServiceRequest request);
    Task DeleteServiceAsync(Guid serviceId);
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

// Provider Auth
public record ProviderRegistrationRequest(
    string FullName,
    string Email,
    string Password,
    string BusinessName,
    string? Description,
    string Phone,
    string? AddressLine1,
    string? AddressLine2,
    string? City,
    string? StateProvince,
    string? PostalCode,
    double? Latitude,
    double? Longitude);

public record ProviderAuthResponse(
    Guid ProfileId,
    Guid ProviderId,
    string FullName,
    string Email,
    string BusinessName,
    Provider? Provider);

// Employee Auth
public record EmployeeAuthResponse(
    Guid StaffId,
    Guid ProviderId,
    string Name,
    string Email,
    string BusinessName,
    StaffMember? Staff);

// Staff Management
public record CreateStaffRequest(
    Guid ProviderId,
    string Name,
    string Email,
    string? Phone,
    string? Bio);

public record UpdateStaffRequest(
    string Name,
    string Email,
    string? Phone,
    string? Bio,
    bool IsActive);

// Schedule Management
public record CreateScheduleRequest(
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime);

public record UpdateScheduleRequest(
    DayOfWeek DayOfWeek,
    TimeOnly StartTime,
    TimeOnly EndTime,
    bool IsActive);

// Service Management
public record CreateServiceRequest(
    Guid ProviderId,
    Guid CategoryId,
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    decimal? DepositRequired,
    string? ImageUrl);

public record UpdateServiceRequest(
    string Name,
    string? Description,
    int DurationMinutes,
    decimal Price,
    decimal? DepositRequired,
    string? ImageUrl,
    bool IsActive);
