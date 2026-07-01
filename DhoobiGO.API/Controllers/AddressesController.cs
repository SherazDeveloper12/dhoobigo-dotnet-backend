using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DhoobiGO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AddressesController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public AddressesController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyAddresses()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var addresses = await _context.UserAddresses
            .Where(a => a.UserId == userId)
            .ToListAsync();
        return Ok(addresses);
    }

    [HttpPost]
    public async Task<IActionResult> AddAddress([FromBody] UserAddress address)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        address.UserId = userId;
        address.Id = 0; // Ensure it's a new record
        
        _context.UserAddresses.Add(address);
        await _context.SaveChangesAsync();
        
        return Ok(address);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAddress(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var address = await _context.UserAddresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        
        if (address == null) return NotFound();

        _context.UserAddresses.Remove(address);
        await _context.SaveChangesAsync();
        
        return Ok(new { Message = "Address deleted successfully" });
    }
}
