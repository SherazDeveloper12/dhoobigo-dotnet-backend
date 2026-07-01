using DhoobiGO.Domain.Common;
using DhoobiGO.Domain.Enums;

namespace DhoobiGO.Domain.Entities;

public class Order : BaseEntity
{
    public string ServiceDescription { get; set; } = string.Empty; // e.g., "10 pieces Washing"
    public int ItemsCount { get; set; }
    public string PickupAddress { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.PendingBidding;
    
    // Foreign Keys
    public int? CustomerId { get; set; }
    public virtual User? Customer { get; set; }
    
    public int? SelectedBidId { get; set; }
    public virtual Bid? SelectedBid { get; set; }
    
    public int? RiderId { get; set; }
    public virtual User? Rider { get; set; }
    public decimal? RiderFee { get; set; } // The negotiated delivery rate
    
    // Navigation
    public virtual ICollection<Bid> Bids { get; set; } = new List<Bid>();
    public virtual ICollection<RiderBid> RiderBids { get; set; } = new List<RiderBid>();
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
    
    public string? ProofImageUrl { get; set; } // Legacy fallback
    public string? PickupProofUrl { get; set; }
    public string? DhobiDropProofUrl { get; set; }
    public string? WashProofUrl { get; set; } // New Slot for Dhobi Finishing Wash
    public string? DhobiPickupProofUrl { get; set; }
    public string? DeliveryProofUrl { get; set; }
    public string? ClothImageUrl { get; set; }
    public bool IsInsured { get; set; } = false;
    public decimal InsuranceFee { get; set; } = 0;
}
