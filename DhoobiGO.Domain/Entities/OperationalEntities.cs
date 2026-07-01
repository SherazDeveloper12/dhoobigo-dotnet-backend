using DhoobiGO.Domain.Common;

namespace DhoobiGO.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public string GroupName { get; set; } = string.Empty; // e.g., "Order-123"
    public string SenderName { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class Review : BaseEntity
{
    public int OrderId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Order Order { get; set; } = null!;

    public int? CustomerId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual User? Customer { get; set; }

    public int? DhobiId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual User? Dhobi { get; set; }

    public int? RiderId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual User? Rider { get; set; }

    public int Rating { get; set; } // 1-5
    public string Comment { get; set; } = string.Empty;
}

public class Wallet : BaseEntity
{
    public decimal Balance { get; set; } = 0;
    public string Currency { get; set; } = "PKR";
    
    public int? UserId { get; set; }
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual User? User { get; set; }

    public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
}

public class WalletTransaction : BaseEntity
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = "Credit"; // Credit or Debit
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    public int WalletId { get; set; }
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Wallet Wallet { get; set; } = null!;
}
