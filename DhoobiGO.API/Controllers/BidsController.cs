using DhoobiGO.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DhoobiGO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BidsController : ControllerBase
{
    private readonly IOrderService _orderService;

    public BidsController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("order/{orderId}")]
    public async Task<IActionResult> GetBidsForOrder(int orderId)
    {
        var result = await _orderService.GetBidsForOrderAsync(orderId);
        return Ok(result);
    }
}
