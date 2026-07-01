using DhoobiGO.Domain.Common;

namespace DhoobiGO.Domain.Entities;

public class Notification : BaseEntity
{
    public int? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "System"; // Order, Wallet, System, Bid
    public bool IsRead { get; set; } = false;
    public string? RelatedEntityId { get; set; } // Reference to OrderId, etc.

    // Navigation
    public virtual User? User { get; set; }
}
