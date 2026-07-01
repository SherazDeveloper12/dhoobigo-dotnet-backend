using System.Security.Claims;
using DhoobiGO.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DhoobiGO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly IUserService _userService;

    public SettingsController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("privacy")]
    public async Task<IActionResult> GetPrivacy()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var prefs = await _userService.GetPreferencesAsync(userId);
        return Ok(prefs);
    }

    [HttpPost("privacy")]
    public async Task<IActionResult> UpdatePrivacy([FromBody] dynamic prefs)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _userService.UpdatePreferencesAsync(userId, prefs);
        return success ? Ok() : BadRequest("Failed to update preferences");
    }
}
