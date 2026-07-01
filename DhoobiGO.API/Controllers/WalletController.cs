using DhoobiGO.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DhoobiGO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public WalletController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetWallet(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.UserId == userId);
            
        if (wallet == null)
        {
            // Auto-create wallet if missing
            wallet = new Domain.Entities.Wallet { UserId = userId, Balance = 500 }; // Sign-up bonus for demo!
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }
            
        return Ok(new {
            balance = wallet.Balance,
            transactions = wallet.Transactions.OrderByDescending(t => t.Timestamp).ToList()
        });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositDto dto)
    {
        var wallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == dto.UserId);
        if (wallet == null) return NotFound();

        wallet.Balance += dto.Amount;
        wallet.Transactions.Add(new Domain.Entities.WalletTransaction
        {
            Amount = dto.Amount,
            Description = "Money Added to Wallet",
            Type = "Credit",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return Ok(new { balance = wallet.Balance });
    }

    public class DepositDto { public int UserId { get; set; } public decimal Amount { get; set; } }
}
