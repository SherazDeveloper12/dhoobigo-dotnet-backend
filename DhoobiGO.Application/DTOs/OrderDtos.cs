using System.Text.Json.Serialization;

namespace DhoobiGO.Application.DTOs;

public class OrderCreateDto
{
    public string ServiceDescription { get; set; } = string.Empty;
    public int ItemsCount { get; set; }
    public string PickupAddress { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int CustomerId { get; set; }
    public string? ClothImageUrl { get; set; }
    public bool IsInsured { get; set; }
}

public class BidCreateDto
{
    public int OrderId { get; set; }
    public int DhobiId { get; set; }
    public decimal Price { get; set; }
    public int DeliveryDays { get; set; }
}

public class OrderResponseDto
{
    public int Id { get; set; }
    public string ServiceDescription { get; set; } = string.Empty;
    public int ItemsCount { get; set; }
    public string PickupAddress { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int BidCount { get; set; }
    public string? DhobiName { get; set; }
    public string? DhobiAddress { get; set; }
    public string? CustomerName { get; set; }
    public double DistanceKm { get; set; }
    public decimal SelectedBidPrice { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TotalAmount { get; set; }
    public double DhobiRating { get; set; }
    public int DhobiReviewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ProofImageUrl { get; set; }
    public string? PickupProofUrl { get; set; }
    public string? DhobiDropProofUrl { get; set; }
    public string? WashProofUrl { get; set; }
    public string? DhobiPickupProofUrl { get; set; }
    public string? DeliveryProofUrl { get; set; }
    public string? ClothImageUrl { get; set; }
    public bool IsInsured { get; set; }
    public decimal InsuranceFee { get; set; }
    public double? RiderLatitude { get; set; }
    public double? RiderLongitude { get; set; }
    public List<BidResponseDto> Bids { get; set; } = new List<BidResponseDto>();
    public List<RiderBidResponseDto> RiderBids { get; set; } = new List<RiderBidResponseDto>();
    public bool IsReviewed { get; set; }
}

public class RiderBidResponseDto
{
    public int Id { get; set; }
    public int RiderId { get; set; }
    public string RiderName { get; set; } = string.Empty;
    public decimal OfferedFee { get; set; }
    public double RiderRating { get; set; }
    public int RiderReviewsCount { get; set; }
    public bool IsAccepted { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class BidResponseDto
{
    public int Id { get; set; }
    public int DhobiId { get; set; }
    public string DhobiName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int DeliveryDays { get; set; }
    public bool IsSelected { get; set; }
    public double DhobiRating { get; set; }
    public int DhobiReviewsCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsPremium { get; set; }
    public bool HasOwnRider { get; set; }
}

public class PaymentRequestDto
{
    public int OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
}

public class UpdateStatusDto
{
    [JsonPropertyName("status")]
    public DhoobiGO.Domain.Enums.OrderStatus Status { get; set; }

    [JsonPropertyName("proofImageUrl")]
    public string? ProofImageUrl { get; set; }
}

public class ReviewCreateDto
{
    public int OrderId { get; set; }
    public int CustomerId { get; set; }
    public int? DhobiId { get; set; }
    public int? RiderId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
