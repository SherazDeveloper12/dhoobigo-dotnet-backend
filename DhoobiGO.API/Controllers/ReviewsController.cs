using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DhoobiGO.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly IReviewService _reviewService;

    public ReviewsController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> PostReview([FromBody] ReviewCreateDto dto)
    {
        dto.CustomerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var success = await _reviewService.SubmitReviewAsync(dto);
        return success ? Ok() : BadRequest("Failed to submit review");
    }
}
