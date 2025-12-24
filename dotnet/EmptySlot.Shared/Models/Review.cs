namespace EmptySlot.Shared.Models;

public class Review
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProviderId { get; set; }
    public Guid? AppointmentId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string? Response { get; set; }
    public DateTime? RespondedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Profile Customer { get; set; } = null!;
    public Provider Provider { get; set; } = null!;
}
