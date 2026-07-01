using DhoobiGO.Application.DTOs;

namespace DhoobiGO.Application.Interfaces;

public interface IUserService
{
    Task<IEnumerable<AuthResponseDto>> GetAllUsersAsync();
    Task<bool> VerifyUserAsync(int id);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> UpdateLocationAsync(int userId, double latitude, double longitude);
    Task<AuthResponseDto?> GetProfileAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId, UpdateProfileDto dto);
    Task<object?> GetPreferencesAsync(int userId);
    Task<bool> UpdatePreferencesAsync(int userId, dynamic prefs);

    // Dhobi Service Portfolio Management
    Task<IEnumerable<DhobiServiceDto>> GetDhobiServicesAsync(int dhobiId);
    Task<bool> AddDhobiServiceAsync(int dhobiId, DhobiServiceCreateDto dto);
    Task<bool> UpdateDhobiServiceAsync(int dhobiId, int serviceId, DhobiServiceUpdateDto dto);
    Task<bool> DeleteDhobiServiceAsync(int dhobiId, int serviceId);
    
    // Tier Upgrades
    Task<bool> RequestTierUpgradeAsync(TierUpgradeDto dto);
    Task<IEnumerable<AuthResponseDto>> GetPendingUpgradesAsync();
    Task<bool> ApproveTierUpgradeAsync(int userId, bool approve);

    // Logistics Linking (Handshake)
    Task<bool> RequestRiderLinkAsync(RiderLinkRequestDto dto);
    Task<IEnumerable<AuthResponseDto>> GetPendingRiderLinksAsync(int dhobiId);
    Task<bool> ConfirmRiderLinkAsync(int dhobiId, int riderId, bool approve);
    Task<IEnumerable<AuthResponseDto>> GetVerifiedDhobisAsync(string search);
}
