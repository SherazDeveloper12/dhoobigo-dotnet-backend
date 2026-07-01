using DhoobiGO.Domain.Common;
using DhoobiGO.Domain.Enums;

namespace DhoobiGO.Domain.Entities;

public class User : BaseEntity
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public double Rating { get; set; } = 5.0;
    public int ReviewsCount { get; set; } = 0;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool IsVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public bool IsDeleted { get; set; } = false;
    
    // Verification Data
    public string? CnicNumber { get; set; }
    public string? CnicImageUrl { get; set; }
    public string? DrivingLicenseUrl { get; set; }
    public string? VehicleRegistrationUrl { get; set; }
    public string? VehicleImageUrl { get; set; }
    public string? VehicleNumber { get; set; }
    public string? SelfieWithIdUrl { get; set; }
    public string? ElectricityBillUrl { get; set; }
    public string? EquipmentImageUrl { get; set; }
    public string? PoliceVerificationUrl { get; set; }
    public string? BusinessLicenseUrl { get; set; }
    public string? ShopName { get; set; }
    public string? FatherName { get; set; }
    public string? Landmark { get; set; }
    public string? NtnNumber { get; set; }
    public string? ReferenceNumbers { get; set; }
    public DhobiType? DhobiType { get; set; }
    public RiderType? RiderType { get; set; }
    public DhobiType? RequestedDhobiType { get; set; }
    public bool IsUpgradePending { get; set; } = false;
    public string? UpgradeDocsUrl { get; set; } // Business license for upgrade
    public DateTime? SubscriptionExpiryDate { get; set; } // End date of premium benefits

    // Rider-Dhobi Handshake (Logistics Linking)
    public int? LinkedDhobiId { get; set; }
    public bool IsLinkVerified { get; set; } = false;

    // Privacy Settings
    public bool PushNotificationsEnabled { get; set; } = true;
    public bool EmailMarketingEnabled { get; set; } = false;
    public bool LocationSharingEnabled { get; set; } = true;
    public bool DataAnalyticsEnabled { get; set; } = true;
    
    // Navigation
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    
    [System.Text.Json.Serialization.JsonIgnore]
    public virtual Wallet? Wallet { get; set; }
    public virtual ICollection<Review> ReviewsAsDhobi { get; set; } = new List<Review>();
    public virtual ICollection<Review> ReviewsAsRider { get; set; } = new List<Review>();
    public virtual ICollection<UserAddress> SavedAddresses { get; set; } = new List<UserAddress>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}
