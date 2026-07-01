using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace DhoobiGO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ReviewController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> PostReview([FromBody] ReviewCreateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);

        var review = new Review
        {
            OrderId = dto.OrderId,
            CustomerId = userId,
            DhobiId = dto.DhobiId,
            RiderId = dto.RiderId,
            Rating = dto.Rating,
            Comment = dto.Comment ?? string.Empty
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        
        // Update User Rating (Average) for Dhobi
        if (review.DhobiId.HasValue && review.DhobiId > 0)
        {
            var dhobi = await _context.Users.FindAsync(review.DhobiId.Value);
            if (dhobi != null)
            {
                var allDhobiReviews = await _context.Reviews
                    .Where(r => r.DhobiId == review.DhobiId.Value)
                    .Select(r => r.Rating)
                    .ToListAsync();
                
                if (allDhobiReviews.Any())
                {
                    dhobi.Rating = allDhobiReviews.Average();
                    dhobi.ReviewsCount = allDhobiReviews.Count;
                }
            }
        }

        // Update User Rating (Average) for Rider
        if (review.RiderId.HasValue && review.RiderId > 0)
        {
            var rider = await _context.Users.FindAsync(review.RiderId.Value);
            if (rider != null)
            {
                var allRiderReviews = await _context.Reviews
                    .Where(r => r.RiderId == review.RiderId.Value)
                    .Select(r => r.Rating)
                    .ToListAsync();
                
                if (allRiderReviews.Any())
                {
                    rider.Rating = allRiderReviews.Average();
                    rider.ReviewsCount = allRiderReviews.Count;
                }
            }
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Review posted successfully" });
    }

    [HttpGet("dhobi/{dhobiId}")]
    public async Task<IActionResult> GetDhobiReviews(int dhobiId)
    {
        var reviews = await _context.Reviews
            .Where(r => r.DhobiId == dhobiId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
            
        return Ok(reviews);
    }
}
