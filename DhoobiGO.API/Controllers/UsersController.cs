using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DhoobiGO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IApplicationDbContext _context;

    public UsersController(IUserService userService, IApplicationDbContext context)
    {
        _userService = userService;
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllUsersAsync();
        return Ok(result);
    }

    [HttpPost("{id}/verify")]
    public async Task<IActionResult> Verify(int id)
    {
        var success = await _userService.VerifyUserAsync(id);
        if (!success) return NotFound();
        return Ok(new { Message = "User verified successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success) return NotFound();
        return Ok(new { Message = "User deleted successfully" });
    }

    [HttpPost("update-location")]
    public async Task<IActionResult> UpdateLocation([FromBody] LocationUpdateDto dto)
    {
        var success = await _userService.UpdateLocationAsync(dto.UserId, dto.Latitude, dto.Longitude);
        if (!success) return NotFound();
        return Ok(new { Message = "Location updated successfully" });
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userId = GetUserId();
        if (userId == 0) return Unauthorized();

        var profile = await _userService.GetProfileAsync(userId);
        if (profile == null) return NotFound();
        
        return Ok(profile);
    }

    [HttpGet("{id}/profile")]
    public async Task<IActionResult> GetPartnerProfile(int id)
    {
        var profile = await _userService.GetProfileAsync(id);
        if (profile == null) return NotFound();
        
        return Ok(profile);
    }

    [HttpPut("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
    {
        var userId = GetUserId();
        if (userId == 0) return Unauthorized();

        var success = await _userService.UpdateProfileAsync(userId, dto);
        if (!success) return NotFound();

        return Ok(new { Message = "Profile updated successfully" });
    }

    // Dhobi Service Management Endpoints
    [HttpGet("services")]
    public async Task<IActionResult> GetMyServices()
    {
        var userId = GetUserId();
        if (userId == 0) return Unauthorized();

        var result = await _userService.GetDhobiServicesAsync(userId);
        return Ok(result);
    }

    [HttpPost("services")]
    public async Task<IActionResult> AddService([FromBody] DhobiServiceCreateDto dto)
    {
        var userId = GetUserId();
        if (userId == 0) return Unauthorized();

        var success = await _userService.AddDhobiServiceAsync(userId, dto);
        return Ok(new { Message = "Service Manifested successfully" });
    }

    [HttpPut("services/{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] DhobiServiceUpdateDto dto)
    {
        var userId = GetUserId();
        if (userId == 0) return Unauthorized();

        var success = await _userService.UpdateDhobiServiceAsync(userId, id, dto);
        if (!success) return NotFound();
        return Ok(new { Message = "Price adjustment Manifested" });
    }

    [HttpDelete("services/{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var userId = GetUserId();
        if (userId == 0) return Unauthorized();

        var success = await _userService.DeleteDhobiServiceAsync(userId, id);
        if (!success) return NotFound();
        return Ok(new { Message = "Service removed successfully" });
    }

    [HttpPost("request-upgrade")]
    public async Task<IActionResult> RequestUpgrade([FromBody] TierUpgradeDto dto)
    {
        var success = await _userService.RequestTierUpgradeAsync(dto);
        if (!success) return BadRequest(new { Message = "Could not submit upgrade request" });
        return Ok(new { Message = "Upgrade request submitted successfully for Admin review" });
    }

    [HttpGet("catalog")]
    public async Task<IActionResult> GetCatalog()
    {
        var result = await _context.ServiceTypes
            .Select(s => new {
                s.Id,
                s.Name,
                s.Description,
                s.Icon,
                s.BasePrice,
                s.Category
            })
            .ToListAsync();
        return Ok(result);
    }

    // Handshake Endpoints
    [AllowAnonymous]
    [HttpGet("dhobis/search")]
    public async Task<IActionResult> SearchDhobis([FromQuery] string? q)
    {
        var result = await _userService.GetVerifiedDhobisAsync(q ?? "");
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("request-rider-link")]
    public async Task<IActionResult> RequestRiderLink([FromBody] RiderLinkRequestDto dto)
    {
        var success = await _userService.RequestRiderLinkAsync(dto);
        if (!success) return BadRequest(new { Message = "Linking failed" });
        return Ok(new { Message = "Link request sent to Dhobi" });
    }

    [HttpGet("rider-link-requests")]
    public async Task<IActionResult> GetRiderLinkRequests()
    {
        var dhobiId = GetUserId();
        var result = await _userService.GetPendingRiderLinksAsync(dhobiId);
        return Ok(result);
    }

    [HttpPost("confirm-rider-link/{riderId}")]
    public async Task<IActionResult> ConfirmRiderLink(int riderId, [FromQuery] bool approve)
    {
        var dhobiId = GetUserId();
        var success = await _userService.ConfirmRiderLinkAsync(dhobiId, riderId, approve);
        if (!success) return NotFound();
        return Ok(new { Message = approve ? "Rider linked successfully" : "Link request rejected" });
    }

    [HttpGet("linked-staff")]
    public async Task<IActionResult> GetLinkedStaff()
    {
        var dhobiId = GetUserId();
        var staff = await _context.Users
            .Where(u => u.LinkedDhobiId == dhobiId && u.IsLinkVerified)
            .Select(u => new {
                u.Id,
                u.FullName,
                u.PhoneNumber,
                Role = u.Role.ToString(),
                u.ProfilePictureUrl,
                u.Rating
            })
            .ToListAsync();
        return Ok(staff);
    }

    private int GetUserId()
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    public class LocationUpdateDto
    {
        public int UserId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
