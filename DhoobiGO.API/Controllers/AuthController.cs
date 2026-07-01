using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DhoobiGO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto);
        if (result == null)
            return BadRequest(new { Message = "User already exists or registration failed." });

        return Ok(result);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try 
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(new { Message = "Invalid email or password." });

            return Ok(result);
        }
        catch (System.UnauthorizedAccessException ex)
        {
            return StatusCode(403, new { Message = ex.Message });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var result = await _authService.ForgotPasswordAsync(email);
        if (!result) return NotFound(new { message = "Email not found" });
        return Ok(new { message = "Reset code sent (check console)" });
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _authService.ResetPasswordAsync(dto);
        if (!result) return BadRequest(new { message = "Failed to reset password" });
        return Ok(new { message = "Password successfully reset" });
    }
}
