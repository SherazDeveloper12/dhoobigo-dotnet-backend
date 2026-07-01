using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DhoobiGO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
    {
        var result = await _orderService.CreateOrderAsync(dto);
        return Ok(result);
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingOrders()
    {
        var result = await _orderService.GetPendingOrdersAsync();
        return Ok(result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyOrders()
    {
        var userId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue("role");

        if (role == "Dhobi")
        {
            var results = await _orderService.GetDhobiOrdersAsync(userId);
            return Ok(results);
        }
        
        if (role == "Rider")
        {
            var results = await _orderService.GetRiderOrdersAsync(userId);
            return Ok(results);
        }

        var customerResults = await _orderService.GetMyOrdersAsync(userId);
        return Ok(customerResults);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id)
    {
        var result = await _orderService.GetOrderByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto dto)
    {
        var success = await _orderService.UpdateOrderStatusAsync(id, dto);
        if (!success) return NotFound();
        return Ok(new { Message = "Status updated successfully" });
    }

    [HttpPost("bid")]
    public async Task<IActionResult> PlaceBid([FromBody] BidCreateDto dto)
    {
        try 
        {
            var success = await _orderService.PlaceBidAsync(dto);
            if (!success) return BadRequest("Unable to place bid. Ensure order is still open for bidding.");
            return Ok(new { Message = "Bid placed successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("{orderId}/select-bid/{bidId}")]
    public async Task<IActionResult> SelectBid(int orderId, int bidId)
    {
        var success = await _orderService.SelectBidAsync(orderId, bidId);
        if (!success) return BadRequest("Failed to select bid. Order may have changed status.");
        return Ok(new { Message = "Partner selected and order lifecycle initiated." });
    }

    [HttpGet("rider/{riderId}")]
    public async Task<IActionResult> GetRiderOrders(int riderId)
    {
        var result = await _orderService.GetRiderOrdersAsync(riderId);
        return Ok(result);
    }

    [HttpGet("dhobi/{dhobiId}")]
    public async Task<IActionResult> GetDhobiOrders(int dhobiId)
    {
        var result = await _orderService.GetDhobiOrdersAsync(dhobiId);
        return Ok(result);
    }

    [HttpGet("available-tasks")]
    public async Task<IActionResult> GetAvailableTasks()
    {
        var result = await _orderService.GetAvailableRiderTasksAsync();
        return Ok(result);
    }

    [HttpPost("{id}/accept-task/{riderId}")]
    public async Task<IActionResult> AcceptTask(int id, int riderId)
    {
        var success = await _orderService.AcceptTaskAsync(id, riderId);
        if (!success) return BadRequest("Failed to accept task. It may have been taken by another rider.");
        return Ok(new { Message = "Task accepted successfully" });
    }

    [HttpPost("bid-delivery")]
    public async Task<IActionResult> BidDelivery([FromBody] RiderBidDto dto)
    {
        try 
        {
            var success = await _orderService.PlaceRiderBidAsync(dto.OrderId, dto.RiderId, dto.Fee);
            if (!success) return BadRequest("Failed to place delivery bid.");
            return Ok(new { Message = "Delivery bid Manifested successfully" });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { Message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("{orderId}/accept-delivery-bid/{bidId}")]
    public async Task<IActionResult> AcceptDeliveryBid(int orderId, int bidId)
    {
        var success = await _orderService.AcceptRiderBidAsync(orderId, bidId);
        if (!success) return BadRequest("Failed to accept delivery bid.");
        return Ok(new { Message = "Delivery bid accepted successfully" });
    }

    public class RiderBidDto
    {
        public int OrderId { get; set; }
        public int RiderId { get; set; }
        public decimal Fee { get; set; }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _orderService.CancelOrderAsync(id);
        if (!result) return BadRequest(new { Message = "Order cannot be cancelled at this stage." });
        return Ok(new { Message = "Order cancelled successfully." });
    }

    [HttpPost("{id}/dispute")]
    public async Task<IActionResult> RaiseDispute(int id, [FromBody] System.Text.Json.JsonElement data)
    {
        string reason = data.GetProperty("reason").GetString() ?? "No reason provided";
        var success = await _orderService.RaiseDisputeAsync(id, reason);
        if (!success) return BadRequest(new { Message = "Failed to raise dispute." });
        return Ok(new { Message = "Dispute raised successfully" });
    }

    [HttpPost("batch-assign")]
    public async Task<IActionResult> BatchAssignRider([FromBody] BatchAssignDto dto)
    {
        var success = await _orderService.BatchAssignRiderAsync(dto.OrderIds, dto.RiderId);
        if (!success) return BadRequest("Some orders could not be assigned.");
        return Ok(new { Message = "Orders assigned in Batch successfully" });
    }

    public class BatchAssignDto { public List<int> OrderIds { get; set; } = new(); public int RiderId { get; set; } }
}
