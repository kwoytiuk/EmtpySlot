using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmptySlot.API.Data;
using EmptySlot.Shared.Models;
using EmptySlot.Shared.Enums;
using System.Security.Claims;

namespace EmptySlot.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public AppointmentsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAppointments()
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

        var appointments = await _context.Appointments
            .Where(a => a.CustomerId == userId)
            .Include(a => a.Provider)
            .Include(a => a.Service)
            .Include(a => a.Location)
            .OrderBy(a => a.AppointmentDate)
            .ThenBy(a => a.StartTime)
            .ToListAsync();

        return Ok(appointments);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            BookingReference = GenerateBookingReference(),
            CustomerId = userId,
            ProviderId = request.ProviderId,
            LocationId = request.LocationId,
            ServiceId = request.ServiceId,
            StaffId = request.StaffId,
            AppointmentDate = request.AppointmentDate,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            Price = request.Price,
            Status = AppointmentStatus.Pending,
            PaymentStatus = PaymentStatus.Pending,
            CustomerNotes = request.CustomerNotes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        return Ok(appointment);
    }

    [HttpGet("available-slots")]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] Guid providerId,
        [FromQuery] Guid serviceId,
        [FromQuery] DateTime date)
    {
        var service = await _context.Services.FindAsync(serviceId);
        if (service == null)
        {
            return NotFound("Service not found");
        }

        var existingAppointments = await _context.Appointments
            .Where(a => a.ProviderId == providerId &&
                       a.AppointmentDate == date.Date &&
                       (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed))
            .ToListAsync();

        var slots = GenerateTimeSlots(date, service.DurationMinutes, existingAppointments);

        return Ok(slots);
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> CancelAppointment(Guid id, [FromBody] CancelRequest request)
    {
        var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "");

        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return NotFound();
        }

        if (appointment.CustomerId != userId)
        {
            return Forbid();
        }

        appointment.Status = AppointmentStatus.Cancelled;
        appointment.CancelledBy = userId;
        appointment.CancelledAt = DateTime.UtcNow;
        appointment.CancellationReason = request.Reason;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(appointment);
    }

    private static string GenerateBookingReference()
    {
        return $"BK{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }

    private static List<TimeSlotResponse> GenerateTimeSlots(
        DateTime date, int durationMinutes, List<Appointment> existingAppointments)
    {
        var slots = new List<TimeSlotResponse>();
        var startHour = 9; // 9 AM
        var endHour = 17; // 5 PM

        for (var hour = startHour; hour < endHour; hour++)
        {
            for (var minute = 0; minute < 60; minute += 30)
            {
                var startTime = new TimeSpan(hour, minute, 0);
                var endTime = startTime.Add(TimeSpan.FromMinutes(durationMinutes));

                var isAvailable = !existingAppointments.Any(a =>
                    (startTime >= a.StartTime && startTime < a.EndTime) ||
                    (endTime > a.StartTime && endTime <= a.EndTime));

                slots.Add(new TimeSlotResponse
                {
                    StartTime = startTime.ToString(@"hh\:mm"),
                    EndTime = endTime.ToString(@"hh\:mm"),
                    Available = isAvailable
                });
            }
        }

        return slots;
    }
}

public record CreateAppointmentRequest(
    Guid ProviderId,
    Guid LocationId,
    Guid ServiceId,
    Guid? StaffId,
    DateTime AppointmentDate,
    TimeSpan StartTime,
    TimeSpan EndTime,
    decimal Price,
    string? CustomerNotes);

public record CancelRequest(string Reason);

public record TimeSlotResponse
{
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public bool Available { get; set; }
}
