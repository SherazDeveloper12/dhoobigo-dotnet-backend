using DhoobiGO.Application.DTOs;

namespace DhoobiGO.Application.Interfaces;

public interface IReviewService
{
    Task<bool> SubmitReviewAsync(ReviewCreateDto dto);
}
