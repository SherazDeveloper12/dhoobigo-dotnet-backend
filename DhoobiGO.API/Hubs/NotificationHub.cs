using Microsoft.AspNetCore.SignalR;

namespace DhoobiGO.API.Hubs;

public class NotificationHub : Hub
{
    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task LeaveGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task NotifyNewOrder()
    {
        await Clients.Group("Dhobis").SendAsync("UpdateMarketplace");
    }

    public async Task NotifyOrderUpdate(int orderId, string status)
    {
        await Clients.Group($"Order_{orderId}").SendAsync("OrderUpdate", orderId, status);
    }
}
