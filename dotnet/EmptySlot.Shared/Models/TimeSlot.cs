namespace EmptySlot.Shared.Models;

/// <summary>
/// Represents an actual bookable time slot for a specific staff member on a specific date
/// </summary>
public class TimeSlot
{
    public Guid Id { get; set; }
    public Guid StaffMemberId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsAvailable { get; set; } = true;
    public Guid? AppointmentId { get; set; } // Null if available, set when booked
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public StaffMember Staff { get; set; } = null!;
    public Appointment? Appointment { get; set; }

    // Computed property for display
    public string DisplayTime => $"{StartTime.ToString("h:mm tt")} - {EndTime.ToString("h:mm tt")}";
}
