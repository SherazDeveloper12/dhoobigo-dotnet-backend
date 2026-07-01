using DhoobiGO.Domain.Common;
using DhoobiGO.Domain.Enums;

namespace DhoobiGO.Domain.Entities;

public class ServiceType : BaseEntity
{
    public string Name { get; set; } = string.Empty; // e.g., "Washing", "Ironing", "Dry Clean"
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public decimal BasePrice { get; set; } // Platform standard/suggested rate
    public string Category { get; set; } = "General"; // e.g., "Cleaning", "Ironing", "Special"
}

public class RiderBid : BaseEntity
{
    public int OrderId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Order Order { get; set; } = null!;

    public int RiderId { get; set; }
    public virtual User Rider { get; set; } = null!;

    public decimal OfferedFee { get; set; }
    public DateTime OfferedAt { get; set; } = DateTime.UtcNow;
    public bool IsAccepted { get; set; } = false;
}

public class Payment : BaseEntity
{
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty; // e.g., "Mock-Visa", "COD"
    public string TransactionId { get; set; } = string.Empty;
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    // Foreign Keys
    public int OrderId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Order Order { get; set; } = null!;
}
