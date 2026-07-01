using DhoobiGO.Domain.Common;

namespace DhoobiGO.Domain.Entities;

public class PaymentMethod : BaseEntity
{
    public int WalletId { get; set; }
    public string Brand { get; set; } = string.Empty; // e.g. Visa, Mastercard
    public string Last4 { get; set; } = string.Empty;
    public string ExpiryDate { get; set; } = string.Empty;
    public bool IsDefault { get; set; } = false;
    
    // Navigation
    public virtual Wallet? Wallet { get; set; }
}
