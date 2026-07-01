using DhoobiGO.Application.DTOs;
using DhoobiGO.Domain.Entities;
using DhoobiGO.Domain.Enums;

namespace DhoobiGO.Application.Interfaces;

public interface IOrderService
{
    Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto);
    Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync();
    Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int customerId);
    Task<IEnumerable<OrderResponseDto>> GetRiderOrdersAsync(int riderId);
    Task<OrderResponseDto?> GetOrderByIdAsync(int id);
    Task<bool> UpdateOrderStatusAsync(int orderId, UpdateStatusDto dto);
    Task<bool> CancelOrderAsync(int orderId);
    Task<bool> RaiseDisputeAsync(int orderId, string reason);
    Task<IEnumerable<BidResponseDto>> GetBidsForOrderAsync(int orderId);
    Task<bool> PlaceBidAsync(BidCreateDto dto);
    Task<bool> SelectBidAsync(int orderId, int bidId);
    
    // Marketplace Logic
    Task<IEnumerable<OrderResponseDto>> GetDhobiOrdersAsync(int dhobiId);
    Task<IEnumerable<OrderResponseDto>> GetAvailableRiderTasksAsync();
    Task<bool> AcceptTaskAsync(int orderId, int riderId);
    Task<bool> PlaceRiderBidAsync(int orderId, int riderId, decimal fee);
    Task<bool> AcceptRiderBidAsync(int orderId, int bidId);
    Task<bool> BatchAssignRiderAsync(List<int> orderIds, int riderId);

    // Mapping helper
    OrderResponseDto MapToResponse(Order order);
}
