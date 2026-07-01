using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;
using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using DhoobiGO.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DhoobiGO.Application.Services;

public class AuthService : IAuthService
{
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(IApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterDto dto)
    {
        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            return null;

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            Role = dto.Role,
            ProfilePictureUrl = dto.ProfilePictureUrl,
            CnicNumber = dto.CnicNumber,
            CnicImageUrl = dto.CnicImageUrl,
            DrivingLicenseUrl = dto.DrivingLicenseUrl,
            VehicleRegistrationUrl = dto.VehicleRegistrationUrl,
            VehicleImageUrl = dto.VehicleImageUrl,
            VehicleNumber = dto.VehicleNumber,
            SelfieWithIdUrl = dto.SelfieWithIdUrl,
            ElectricityBillUrl = dto.ElectricityBillUrl,
            EquipmentImageUrl = dto.EquipmentImageUrl,
            PoliceVerificationUrl = dto.PoliceVerificationUrl,
            BusinessLicenseUrl = dto.BusinessLicenseUrl,
            ShopName = dto.ShopName,
            FatherName = dto.FatherName,
            Landmark = dto.Landmark,
            NtnNumber = dto.NtnNumber,
            ReferenceNumbers = dto.ReferenceNumbers,
            DhobiType = dto.DhobiType.HasValue ? (DhobiType)dto.DhobiType.Value : null,
            RiderType = dto.RiderType.HasValue ? (RiderType)dto.RiderType.Value : null,
            IsVerified = dto.Role == UserRole.Customer
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Rating = user.Rating,
            ReviewsCount = user.ReviewsCount,
            DhobiType = user.DhobiType != null ? (int)user.DhobiType : null,
            Token = GenerateJwtToken(user)
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return null;

        // VERIFICATION GATE: Check if Dhobi/Rider is verified by Admin
        if (user.Role != UserRole.Customer && user.Role != UserRole.Admin && !user.IsVerified)
        {
            throw new UnauthorizedAccessException("Account pending administrative approval. Please wait for verification.");
        }

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            Rating = user.Rating,
            ReviewsCount = user.ReviewsCount,
            DhobiType = user.DhobiType != null ? (int)user.DhobiType : null,
            Token = GenerateJwtToken(user)
        };
    }

    public async Task<bool> ForgotPasswordAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;

        // Generate a simple mock reset code
        var resetCode = new Random().Next(100000, 999999).ToString();
        
        // In a real app, send email here. For DhoobiGO, we print to console.
        Console.WriteLine($"\n[MOCK EMAIL] To: {email} | Reset Code: {resetCode}\n");
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null) return false;

        // For mock reset, we just accept any 6 digit code for now, or match a static one
        // In this implementation, we simply hash the new password.
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    private string GenerateJwtToken(User user)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secret = jwtSettings["Secret"] ?? "a_very_long_and_secure_secret_that_is_at_least_32_characters_long";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()), // Standard Claim for Authorize attribute
            new Claim("role", user.Role.ToString()), // Legacy short-form for frontend
            new Claim("name", user.FullName)
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "DhoobiGO",
            audience: jwtSettings["Audience"] ?? "DhoobiGO_Users",
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
