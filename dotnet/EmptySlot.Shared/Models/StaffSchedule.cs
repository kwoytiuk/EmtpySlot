namespace EmptySlot.Shared.Models;

/// <summary>
/// Represents a staff member's recurring weekly schedule
/// </summary>
public class StaffSchedule
{
    public Guid Id { get; set; }
    public Guid StaffMemberId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public StaffMember Staff { get; set; } = null!;
}
