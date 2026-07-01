using DhoobiGO.Domain.Enums;

namespace DhoobiGO.Application.DTOs;

public class RegisterDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public string? ProfilePictureUrl { get; set; }
    
    // Optional Professional Fields
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
    public int? DhobiType { get; set; }
    public int? RiderType { get; set; }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
{
    public int UserId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public string Token { get; set; } = string.Empty;
    public bool IsVerified { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Extended Info for Admin Audit
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
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
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double Rating { get; set; }
    public int ReviewsCount { get; set; }
    public int CompletedJobsCount { get; set; }
    public bool IsUpgradePending { get; set; }
    public int? DhobiType { get; set; }
}

public class ResetPasswordDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}

public class TierUpgradeDto
{
    public int UserId { get; set; }
    public int RequestedType { get; set; }
    public string? ShopName { get; set; }
    public string? NtnNumber { get; set; }
    public string? UpgradeDocsUrl { get; set; }
}

public class RiderLinkRequestDto
{
    public int RiderId { get; set; }
    public int DhobiId { get; set; }
}
