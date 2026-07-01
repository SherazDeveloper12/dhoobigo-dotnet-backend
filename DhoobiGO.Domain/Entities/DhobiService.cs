using DhoobiGO.Domain.Common;

namespace DhoobiGO.Domain.Entities;

public class DhobiService : BaseEntity
{
    public int DhobiId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual User Dhobi { get; set; } = null!;

    public int ServiceTypeId { get; set; }
    public virtual ServiceType ServiceType { get; set; } = null!;

    public decimal Price { get; set; } // e.g., 50.00 Rs
    public string Unit { get; set; } = "pc"; // pc, kg, sqft
    public bool IsActive { get; set; } = true;
}
