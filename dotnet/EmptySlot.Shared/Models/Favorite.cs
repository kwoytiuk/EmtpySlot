namespace EmptySlot.Shared.Models;

public class Favorite
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ProviderId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Profile Customer { get; set; } = null!;
}
