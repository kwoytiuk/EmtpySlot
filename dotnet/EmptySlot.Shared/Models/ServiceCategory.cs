namespace EmptySlot.Shared.Models;

public class ServiceCategory
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Icon { get; set; }
    public string? Description { get; set; }
    public Guid? ParentId { get; set; }
    public int DisplayOrder { get; set; } = 0;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ServiceCategory? Parent { get; set; }
    public ICollection<ServiceCategory> Children { get; set; } = new List<ServiceCategory>();
    public ICollection<Service> Services { get; set; } = new List<Service>();
}
