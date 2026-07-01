using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using DhoobiGO.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DhoobiGO.Application.Services;

public class UserService : IUserService
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;

    public UserService(IApplicationDbContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<IEnumerable<AuthResponseDto>> GetAllUsersAsync()
    {
        var users = await _context.Users
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(u => new AuthResponseDto
        {
            UserId = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role.ToString(),
            ProfilePictureUrl = u.ProfilePictureUrl,
            IsVerified = u.IsVerified,
            CreatedAt = u.CreatedAt,
            PhoneNumber = u.PhoneNumber,
            Address = u.Address,
            CnicNumber = u.CnicNumber,
            CnicImageUrl = u.CnicImageUrl,
            DrivingLicenseUrl = u.DrivingLicenseUrl,
            VehicleRegistrationUrl = u.VehicleRegistrationUrl,
            VehicleImageUrl = u.VehicleImageUrl,
            VehicleNumber = u.VehicleNumber,
            SelfieWithIdUrl = u.SelfieWithIdUrl,
            ElectricityBillUrl = u.ElectricityBillUrl,
            EquipmentImageUrl = u.EquipmentImageUrl,
            PoliceVerificationUrl = u.PoliceVerificationUrl,
            BusinessLicenseUrl = u.BusinessLicenseUrl,
            ShopName = u.ShopName,
            Latitude = u.Latitude,
            Longitude = u.Longitude,
            Rating = u.Rating,
            ReviewsCount = u.ReviewsCount,
            CompletedJobsCount = _context.Orders.Count(o => (o.RiderId == u.Id || (o.SelectedBid != null && o.SelectedBid.DhobiId == u.Id)) && o.Status == OrderStatus.Completed),
            IsUpgradePending = u.IsUpgradePending,
            DhobiType = u.DhobiType != null ? (int)u.DhobiType : null,
            Token = "" 
        });
    }

    public async Task<bool> VerifyUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsVerified = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsDeleted = true;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateLocationAsync(int userId, double latitude, double longitude)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.Latitude = latitude;
        user.Longitude = longitude;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<AuthResponseDto?> GetProfileAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsVerified = user.IsVerified,
            CreatedAt = user.CreatedAt,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            CnicNumber = user.CnicNumber,
            CnicImageUrl = user.CnicImageUrl,
            DrivingLicenseUrl = user.DrivingLicenseUrl,
            VehicleRegistrationUrl = user.VehicleRegistrationUrl,
            VehicleImageUrl = user.VehicleImageUrl,
            VehicleNumber = user.VehicleNumber,
            SelfieWithIdUrl = user.SelfieWithIdUrl,
            ElectricityBillUrl = user.ElectricityBillUrl,
            EquipmentImageUrl = user.EquipmentImageUrl,
            PoliceVerificationUrl = user.PoliceVerificationUrl,
            BusinessLicenseUrl = user.BusinessLicenseUrl,
            ShopName = user.ShopName,
            Latitude = user.Latitude,
            Longitude = user.Longitude,
            Rating = user.Rating,
            ReviewsCount = user.ReviewsCount,
            CompletedJobsCount = await _context.Orders.CountAsync(o => (o.RiderId == userId || (o.SelectedBid != null && o.SelectedBid.DhobiId == userId)) && o.Status == OrderStatus.Completed),
            IsUpgradePending = user.IsUpgradePending,
            DhobiType = user.DhobiType != null ? (int)user.DhobiType : null,
            Token = "" 
        };
    }

    public async Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto dto)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        user.FullName = dto.FullName;
        user.PhoneNumber = dto.PhoneNumber;
        user.Address = dto.Address;
        user.Latitude = dto.Latitude ?? user.Latitude;
        user.Longitude = dto.Longitude ?? user.Longitude;

        if (!string.IsNullOrEmpty(dto.ProfilePictureUrl))
        {
            user.ProfilePictureUrl = dto.ProfilePictureUrl;
        }
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<object?> GetPreferencesAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new
        {
            user.PushNotificationsEnabled,
            user.EmailMarketingEnabled,
            user.LocationSharingEnabled,
            user.DataAnalyticsEnabled
        };
    }

    public async Task<bool> UpdatePreferencesAsync(int userId, dynamic prefs)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        // Using dynamic to flexibly handle partial updates from mobile
        if (prefs.pushNotifications != null) user.PushNotificationsEnabled = prefs.pushNotifications;
        if (prefs.emailMarketing != null) user.EmailMarketingEnabled = prefs.emailMarketing;
        if (prefs.locationSharing != null) user.LocationSharingEnabled = prefs.locationSharing;
        if (prefs.dataAnalytics != null) user.DataAnalyticsEnabled = prefs.dataAnalytics;

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<DhobiServiceDto>> GetDhobiServicesAsync(int dhobiId)
    {
        return await _context.DhobiServices
            .Where(s => s.DhobiId == dhobiId)
            .Include(s => s.ServiceType)
            .Select(s => new DhobiServiceDto
            {
                Id = s.Id,
                ServiceTypeId = s.ServiceTypeId,
                ServiceName = s.ServiceType.Name,
                ServiceIcon = s.ServiceType.Icon,
                Price = s.Price,
                Unit = s.Unit,
                IsActive = s.IsActive
            })
            .ToListAsync();
    }

    public async Task<bool> AddDhobiServiceAsync(int dhobiId, DhobiServiceCreateDto dto)
    {
        var service = new DhobiService
        {
            DhobiId = dhobiId,
            ServiceTypeId = dto.ServiceTypeId,
            Price = dto.Price,
            Unit = dto.Unit,
            IsActive = true
        };

        _context.DhobiServices.Add(service);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateDhobiServiceAsync(int dhobiId, int serviceId, DhobiServiceUpdateDto dto)
    {
        var service = await _context.DhobiServices
            .FirstOrDefaultAsync(s => s.Id == serviceId && s.DhobiId == dhobiId);
            
        if (service == null) return false;

        service.Price = dto.Price;
        service.IsActive = dto.IsActive;
        service.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDhobiServiceAsync(int dhobiId, int serviceId)
    {
        var service = await _context.DhobiServices.FindAsync(serviceId);
        if (service == null || service.DhobiId != dhobiId) return false;

        _context.DhobiServices.Remove(service);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> RequestTierUpgradeAsync(TierUpgradeDto dto)
    {
        var user = await _context.Users.FindAsync(dto.UserId);
        if (user == null) return false;

        user.RequestedDhobiType = (DhobiType)dto.RequestedType;
        user.IsUpgradePending = true;
        user.ShopName = dto.ShopName ?? user.ShopName;
        user.NtnNumber = dto.NtnNumber ?? user.NtnNumber;
        user.UpgradeDocsUrl = dto.UpgradeDocsUrl;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AuthResponseDto>> GetPendingUpgradesAsync()
    {
        var users = await _context.Users
            .Where(u => u.IsUpgradePending)
            .ToListAsync();

        return users.Select(u => new AuthResponseDto
        {
            UserId = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            ShopName = u.ShopName,
            BusinessLicenseUrl = u.UpgradeDocsUrl, // Map upgrade doc for admin review
            Role = u.RequestedDhobiType.ToString(), // Show what they want to become
            IsVerified = u.IsVerified,
            CreatedAt = u.CreatedAt
        });
    }

    public async Task<bool> ApproveTierUpgradeAsync(int userId, bool approve)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (approve && user.RequestedDhobiType.HasValue)
        {
            user.DhobiType = user.RequestedDhobiType.Value;
            
            // Set 30-day subscription if upgrading to Premium
            if (user.RequestedDhobiType == DhobiType.Premium)
            {
                user.SubscriptionExpiryDate = DateTime.UtcNow.AddDays(30);
            }
        }

        user.IsUpgradePending = false;
        user.RequestedDhobiType = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        // Notify user about status change
        await _notificationService.NotifyUser(userId, approve ? "Account upgraded successfully!" : "Upgrade request rejected", "UpgradeUpdate");
        
        return true;
    }

    public async Task<bool> RequestRiderLinkAsync(RiderLinkRequestDto dto)
    {
        var rider = await _context.Users.FindAsync(dto.RiderId);
        if (rider == null) return false;

        rider.LinkedDhobiId = dto.DhobiId;
        rider.IsLinkVerified = false;
        rider.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AuthResponseDto>> GetPendingRiderLinksAsync(int dhobiId)
    {
        var riders = await _context.Users
            .Where(u => u.LinkedDhobiId == dhobiId && !u.IsLinkVerified)
            .ToListAsync();

        return riders.Select(u => new AuthResponseDto
        {
            UserId = u.Id,
            FullName = u.FullName,
            Email = u.Email,
            PhoneNumber = u.PhoneNumber,
            ProfilePictureUrl = u.ProfilePictureUrl,
            Role = "Linked Rider"
        });
    }

    public async Task<bool> ConfirmRiderLinkAsync(int dhobiId, int riderId, bool approve)
    {
        var rider = await _context.Users.FindAsync(riderId);
        if (rider == null || rider.LinkedDhobiId != dhobiId) return false;

        if (approve)
        {
            rider.IsLinkVerified = true;
        }
        else
        {
            rider.LinkedDhobiId = null;
            rider.IsLinkVerified = false;
        }

        rider.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<AuthResponseDto>> GetVerifiedDhobisAsync(string search)
    {
        var query = _context.Users
            .Where(u => u.Role == UserRole.Dhobi && u.IsVerified);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(u => u.ShopName != null && u.ShopName.Contains(search));
        }

        var dhobis = await query.ToListAsync();

        return dhobis.Select(u => new AuthResponseDto
        {
            UserId = u.Id,
            FullName = u.FullName,
            ShopName = u.ShopName ?? u.FullName,
            Address = u.Address,
            ProfilePictureUrl = u.ProfilePictureUrl
        });
    }
}
