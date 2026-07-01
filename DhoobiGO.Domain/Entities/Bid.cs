using DhoobiGO.Domain.Common;

namespace DhoobiGO.Domain.Entities;

public class Bid : BaseEntity
{
    public decimal Price { get; set; }
    public int DeliveryDays { get; set; }
    public bool IsSelected { get; set; } = false;
    
    public int OrderId { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Order Order { get; set; } = null!;
    
    public int? DhobiId { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual User? Dhobi { get; set; }
}
