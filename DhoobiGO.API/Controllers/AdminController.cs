using DhoobiGO.Application.Interfaces;
using DhoobiGO.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DhoobiGO.Application.DTOs;

namespace DhoobiGO.API.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class AdminController : ControllerBase
{
    private readonly IApplicationDbContext _context;
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;

    public AdminController(IApplicationDbContext context, IOrderService orderService, IUserService userService)
    {
        _context = context;
        _orderService = orderService;
        _userService = userService;
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var totalUsers = await _context.Users.CountAsync();
        var totalOrders = await _context.Orders.CountAsync();
        var activeOrders = await _context.Orders.CountAsync(o => o.Status != DhoobiGO.Domain.Enums.OrderStatus.Completed);
        var totalBids = await _context.Bids.CountAsync();
        
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Bids)
                .ThenInclude(b => b.Dhobi)
            .OrderByDescending(o => o.CreatedAt)
            .Take(5)
            .ToListAsync();

        var recentOrders = orders.Select(_orderService.MapToResponse);

        return Ok(new
        {
            TotalUsers = totalUsers,
            TotalOrders = totalOrders,
            ActiveOrders = activeOrders,
            TotalBids = totalBids,
            RecentOrders = recentOrders
        });
    }

    [HttpPost("verify-user/{id}")]
    public async Task<IActionResult> VerifyUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.IsVerified = true;
        await _context.SaveChangesAsync();
        return Ok(new { Message = "User verified successfully." });
    }

    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return Ok(new { Message = "User deleted successfully." });
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetAllOrders()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Bids)
                .ThenInclude(b => b.Dhobi)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        var result = orders.Select(_orderService.MapToResponse);
        return Ok(result);
    }

    [HttpPost("assign-rider")]
    public async Task<IActionResult> AssignRider([FromBody] AssignRiderDto dto)
    {
        var order = await _context.Orders.FindAsync(dto.OrderId);
        if (order == null) return NotFound();

        order.RiderId = dto.RiderId;
        if (order.Status == DhoobiGO.Domain.Enums.OrderStatus.BidSelected)
            order.Status = DhoobiGO.Domain.Enums.OrderStatus.PickupScheduled;

        await _context.SaveChangesAsync();
        return Ok(new { Message = "Rider assigned manually by Admin." });
    }

    [HttpGet("pending-upgrades")]
    public async Task<IActionResult> GetPendingUpgrades()
    {
        var result = await _userService.GetPendingUpgradesAsync();
        return Ok(result);
    }

    [HttpPost("approve-upgrade/{id}")]
    public async Task<IActionResult> ApproveUpgrade(int id, [FromQuery] bool approve)
    {
        var success = await _userService.ApproveTierUpgradeAsync(id, approve);
        if (!success) return NotFound();
        return Ok(new { Message = approve ? "User upgraded successfully" : "Upgrade request rejected" });
    }

    [Authorize]
    [HttpGet("test-auth")]
    public IActionResult TestAuth()
    {
        return Ok(new
        {
            User = User.Identity?.Name,
            AuthenticationType = User.Identity?.AuthenticationType,
            Claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList(),
            IsInAdminRole = User.IsInRole("Admin")
        });
    }

    public class AssignRiderDto { public int OrderId { get; set; } public int RiderId { get; set; } }
}
