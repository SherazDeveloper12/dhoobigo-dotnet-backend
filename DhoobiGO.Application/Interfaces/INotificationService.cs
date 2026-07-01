namespace DhoobiGO.Application.Interfaces;

public interface INotificationService
{
    Task NotifyNewOrder();
    Task NotifyNewBid(int orderId, int userId);
    Task NotifyOrderUpdate(int orderId, string status);
    Task NotifyUser(int userId, string message, string type);
}
