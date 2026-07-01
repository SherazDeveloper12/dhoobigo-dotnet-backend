using DhoobiGO.Domain.Common;

namespace DhoobiGO.Domain.Entities;

public class UserAddress : BaseEntity
{
    public int? UserId { get; set; }
    public string Label { get; set; } = "Home"; // Home, Work, etc.
    public string AddressLine { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public bool IsDefault { get; set; } = false;

    // Navigation
    public virtual User? User { get; set; }
}
