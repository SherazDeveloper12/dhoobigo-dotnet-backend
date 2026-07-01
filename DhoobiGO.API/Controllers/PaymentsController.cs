using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DhoobiGO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var balance = await _paymentService.GetWalletBalanceAsync(userId);
        return Ok(new { balance });
    }

    [HttpGet("wallet")]
    public async Task<IActionResult> GetWalletOverview()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _paymentService.GetWalletAsync(userId);
        return Ok(result);
    }

    [HttpGet("wallet/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetWalletById(int userId)
    {
        var result = await _paymentService.GetWalletAsync(userId);
        return Ok(result);
    }

    [HttpGet("methods")]
    public async Task<IActionResult> GetMethods()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var methods = await _paymentService.GetSavedCardsAsync(userId);
        return Ok(methods);
    }

    [HttpPost("methods")]
    public async Task<IActionResult> AddMethod([FromBody] AddPaymentMethodDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _paymentService.AddPaymentMethodAsync(userId, dto);
        return success ? Ok() : BadRequest("Failed to add payment method");
    }

    [HttpPost("topup")]
    public async Task<IActionResult> TopUp([FromBody] TopUpDto dto)
    {
        var currentUserIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserIdString == null) return Unauthorized();
        
        var currentUserId = int.Parse(currentUserIdString);
        var role = User.FindFirstValue(ClaimTypes.Role);

        // If UserId is specified in DTO and requester is Admin, use specified UserId
        var targetUserId = (dto.UserId.HasValue && role == "Admin") ? dto.UserId.Value : currentUserId;
        
        var description = dto.Description ?? "Wallet Top Up";
        var success = await _paymentService.AdjustBalanceAsync(targetUserId, dto.Amount, description);
        
        return success ? Ok() : BadRequest("Failed to top up wallet");
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawalRequestDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _paymentService.CreateWithdrawalRequestAsync(userId, dto.Amount, dto.Method);
        if (!success) return BadRequest("Insufficient balance or invalid request.");
        return Ok(new { Message = "Withdrawal request submitted successfully" });
    }

    [HttpPost]
    public async Task<IActionResult> Process([FromBody] PaymentRequestDto dto)
    {
        var success = await _paymentService.ProcessPaymentAsync(dto);
        if (!success) return BadRequest("Failed to process payment.");
        return Ok(new { Message = "Payment recorded successfully." });
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetStatus(int orderId)
    {
        var result = await _paymentService.GetPaymentStatusAsync(orderId);
        if (result == null) return NotFound();
        return Ok(result);
    }
}
