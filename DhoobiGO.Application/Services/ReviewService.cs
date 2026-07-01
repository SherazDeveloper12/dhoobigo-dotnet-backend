using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DhoobiGO.Application.Services;

public class ReviewService : IReviewService
{
    private readonly IApplicationDbContext _context;

    public ReviewService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> SubmitReviewAsync(ReviewCreateDto dto)
    {
        var review = new Review
        {
            OrderId = dto.OrderId,
            CustomerId = dto.CustomerId,
            DhobiId = dto.DhobiId,
            RiderId = dto.RiderId,
            Rating = dto.Rating,
            Comment = dto.Comment ?? string.Empty
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        // Update Average Ratings
        if (dto.DhobiId.HasValue)
        {
            await UpdateUserRatingAsync(dto.DhobiId.Value);
        }
        if (dto.RiderId.HasValue)
        {
            await UpdateUserRatingAsync(dto.RiderId.Value);
        }

        return true;
    }

    private async Task UpdateUserRatingAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return;

        var reviews = await _context.Reviews
            .Where(r => r.DhobiId == userId || r.RiderId == userId)
            .ToListAsync();

        if (reviews.Any())
        {
            user.Rating = reviews.Average(r => r.Rating);
            user.ReviewsCount = reviews.Count;
            await _context.SaveChangesAsync();
        }
    }
}
