using EmptySlot.Shared.Enums;

namespace EmptySlot.Shared.Models;

public class Appointment
{
    public Guid Id { get; set; }
    public string BookingReference { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid LocationId { get; set; }
    public Guid ServiceId { get; set; }
    public Guid? StaffId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime StartDateTime { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public decimal Price { get; set; }
    public decimal? DepositPaid { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
    public string? PaymentIntentId { get; set; }
    public string? CustomerNotes { get; set; }
    public string? ProviderNotes { get; set; }
    public Guid? CancelledBy { get; set; }
    public DateTime? CancelledAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Profile Customer { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
    public ProviderLocation Location { get; set; } = null!;
    public Service Service { get; set; } = null!;
    public StaffMember? Staff { get; set; }
}
