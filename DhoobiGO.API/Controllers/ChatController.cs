using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using DhoobiGO.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DhoobiGO.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IApplicationDbContext _context;

    public ChatController(IApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("history/{groupName}")]
    public async Task<IActionResult> GetChatHistory(string groupName)
    {
        var messages = await _context.ChatMessages
            .Where(m => m.GroupName == groupName)
            .OrderBy(m => m.Timestamp)
            .ToListAsync();
            
        return Ok(messages);
    }

    [HttpGet("active-groups")]
    public async Task<IActionResult> GetActiveGroups()
    {
        var groups = await _context.ChatMessages
            .GroupBy(m => m.GroupName)
            .Select(g => new
            {
                GroupName = g.Key,
                LastMessageAt = g.Max(m => m.Timestamp),
                MessageCount = g.Count()
            })
            .OrderByDescending(g => g.LastMessageAt)
            .ToListAsync();

        return Ok(groups);
    }

    [HttpGet("conversations/{userId}")]
    public async Task<IActionResult> GetConversations(int userId)
    {
        // Find orders where the user is Customer, Dhobi, or Rider
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.SelectedBid).ThenInclude(b => b.Dhobi)
            .Include(o => o.RiderBids)
            .Where(o => o.CustomerId == userId 
                     || (o.SelectedBid != null && o.SelectedBid.DhobiId == userId)
                     || (o.RiderBids.Any(rb => rb.IsAccepted && rb.RiderId == userId)))
            .OrderByDescending(o => o.Id)
            .ToListAsync();

        var conversations = new List<object>();

        foreach (var order in orders)
        {
            // Case 1: User is the Customer
            if (order.CustomerId == userId)
            {
                // Customer -> Dhobi conversation
                if (order.SelectedBid?.Dhobi != null)
                {
                    var dhobiGroup = $"{order.Id}_Dhobi";
                    var lastMsg = await _context.ChatMessages
                        .Where(m => m.GroupName == dhobiGroup || m.GroupName == order.Id.ToString()) // Legacy support
                        .OrderByDescending(m => m.Timestamp)
                        .FirstOrDefaultAsync();

                    conversations.Add(new {
                        id = dhobiGroup,
                        orderId = order.Id.ToString(),
                        name = order.SelectedBid.Dhobi.FullName,
                        role = "Dhobi",
                        initial = order.SelectedBid.Dhobi.FullName?[0].ToString() ?? "D",
                        lastMsg = lastMsg?.Message ?? "Tap to chat with Dhobi...",
                        time = lastMsg?.Timestamp.ToLocalTime().ToString("hh:mm tt") ?? ""
                    });
                }

                // Customer -> Rider conversation
                var acceptedRiderBid = order.RiderBids.FirstOrDefault(rb => rb.IsAccepted);
                if (acceptedRiderBid != null)
                {
                    var rider = await _context.Users.FindAsync(acceptedRiderBid.RiderId);
                    if (rider != null)
                    {
                        var riderGroup = $"{order.Id}_Rider";
                        var lastMsg = await _context.ChatMessages
                            .Where(m => m.GroupName == riderGroup)
                            .OrderByDescending(m => m.Timestamp)
                            .FirstOrDefaultAsync();

                        conversations.Add(new {
                            id = riderGroup,
                            orderId = order.Id.ToString(),
                            name = rider.FullName,
                            role = "Rider",
                            initial = rider.FullName?[0].ToString() ?? "R",
                            lastMsg = lastMsg?.Message ?? "Tap to chat with Rider...",
                            time = lastMsg?.Timestamp.ToLocalTime().ToString("hh:mm tt") ?? ""
                        });
                    }
                }
            }
            // Case 2: User is the Dhobi
            else if (order.SelectedBid?.DhobiId == userId)
            {
                var dhobiGroup = $"{order.Id}_Dhobi";
                var lastMsg = await _context.ChatMessages
                    .Where(m => m.GroupName == dhobiGroup || m.GroupName == order.Id.ToString())
                    .OrderByDescending(m => m.Timestamp)
                    .FirstOrDefaultAsync();

                conversations.Add(new {
                    id = dhobiGroup,
                    orderId = order.Id.ToString(),
                    name = order.Customer?.FullName ?? "Customer",
                    role = "Customer",
                    initial = order.Customer?.FullName?[0].ToString() ?? "C",
                    lastMsg = lastMsg?.Message ?? "Tap to chat with Customer...",
                    time = lastMsg?.Timestamp.ToLocalTime().ToString("hh:mm tt") ?? ""
                });
            }
            // Case 3: User is the Rider
            else 
            {
                var riderGroup = $"{order.Id}_Rider";
                var lastMsg = await _context.ChatMessages
                    .Where(m => m.GroupName == riderGroup)
                    .OrderByDescending(m => m.Timestamp)
                    .FirstOrDefaultAsync();

                conversations.Add(new {
                    id = riderGroup,
                    orderId = order.Id.ToString(),
                    name = order.Customer?.FullName ?? "Customer",
                    role = "Customer",
                    initial = order.Customer?.FullName?[0].ToString() ?? "C",
                    lastMsg = lastMsg?.Message ?? "Tap to chat with Customer...",
                    time = lastMsg?.Timestamp.ToLocalTime().ToString("hh:mm tt") ?? ""
                });
            }
        }

        return Ok(conversations);
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendMessage([FromBody] MessageCreateDto dto)
    {
        var chatMessage = new ChatMessage
        {
            GroupName = dto.GroupName,
            SenderName = dto.SenderName,
            Message = dto.Message,
            Timestamp = DateTime.UtcNow
        };

        _context.ChatMessages.Add(chatMessage);
        await _context.SaveChangesAsync();
        
        return Ok(chatMessage);
    }

    public class MessageCreateDto
    {
        public string GroupName { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
