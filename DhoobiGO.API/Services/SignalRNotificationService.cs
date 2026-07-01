using DhoobiGO.Application.Interfaces;
using DhoobiGO.API.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace DhoobiGO.API.Services;

public class SignalRNotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyNewOrder()
    {
        // Only broadcast to the "Dhobis" group
        await _hubContext.Clients.Group("Dhobis").SendAsync("UpdateMarketplace");
    }

    public async Task NotifyNewBid(int orderId, int userId)
    {
        // Notify the specific customer via their personal group
        await _hubContext.Clients.Group($"User_{userId}").SendAsync("NewBid", orderId);
        
        // Also notify anyone actively watching the order screen
        await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("NewBid", orderId);
    }

    public async Task NotifyOrderUpdate(int orderId, string status)
    {
        // Only broadcast to the participants of this specific order
        await _hubContext.Clients.Group($"Order_{orderId}").SendAsync("OrderUpdate", orderId, status);
    }

    public async Task NotifyUser(int userId, string message, string type)
    {
        await _hubContext.Clients.Group($"User_{userId}").SendAsync("PersonalNotification", message, type);
    }
}
