namespace EmptySlot.Shared.Models;

public class Provider
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string BusinessName { get; set; } = "";
    public string? Description { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? CoverImageUrl { get; set; }
    public bool Verified { get; set; } = false;
    public decimal RatingAverage { get; set; } = 0;
    public int RatingCount { get; set; } = 0;
    public string? CancellationPolicy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Profile User { get; set; } = null!;
    public ICollection<ProviderLocation> Locations { get; set; } = new List<ProviderLocation>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
    public ICollection<StaffMember> StaffMembers { get; set; } = new List<StaffMember>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}
