using Microsoft.AspNetCore.SignalR;
using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;

namespace DhoobiGO.API.Hubs;

public class ChatHub : Hub
{
    private readonly IApplicationDbContext _context;

    public ChatHub(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task JoinGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendMessageToGroup(string groupName, string user, string message)
    {
        // Persistence: Save to Database
        var chatMsg = new ChatMessage
        {
            GroupName = groupName,
            SenderName = user,
            Message = message,
            Timestamp = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMsg);
        await _context.SaveChangesAsync();

        await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
    }
}
