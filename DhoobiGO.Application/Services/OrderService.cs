using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using DhoobiGO.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DhoobiGO.Application.Interfaces;
namespace DhoobiGO.Application.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationService _notificationService;
    private readonly IUserService _userService;

    public OrderService(IApplicationDbContext context, IUserService userService, INotificationService notificationService)
    {
        _context = context;
        _userService = userService;
        _notificationService = notificationService;
    }

    public async Task<OrderResponseDto> CreateOrderAsync(OrderCreateDto dto)
    {
        var order = new Order
        {
            ServiceDescription = dto.ServiceDescription,
            ItemsCount = dto.ItemsCount,
            PickupAddress = dto.PickupAddress,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude,
            CustomerId = dto.CustomerId,
            ClothImageUrl = dto.ClothImageUrl,
            IsInsured = dto.IsInsured,
            InsuranceFee = dto.IsInsured ? 30 : 0,
            Status = OrderStatus.PendingBidding,
            CreatedAt = DateTime.UtcNow
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Real-time: Notify all Dhobis of a new order
        await _notificationService.NotifyNewOrder();

        return MapToResponse(order);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetPendingOrdersAsync()
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Bids)
            .Where(o => o.Status == OrderStatus.PendingBidding)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponse);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetMyOrdersAsync(int userId)
    {
        var orders = await _context.Orders
            .Include(o => o.Bids)
                .ThenInclude(b => b.Dhobi)
            .Include(o => o.Customer)
            .Include(o => o.RiderBids)
            .Include(o => o.Reviews)
            .Where(o => o.CustomerId == userId || 
                       o.Bids.Any(b => b.DhobiId == userId && b.IsSelected) ||
                       o.RiderId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponse);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetRiderOrdersAsync(int riderId)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Bids)
                .ThenInclude(b => b.Dhobi)
            .Where(o => o.RiderId == riderId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return orders.Select(MapToResponse);
    }

    public async Task<OrderResponseDto?> GetOrderByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.SelectedBid)
                .ThenInclude(sb => sb.Dhobi)
            .Include(o => o.Bids)
                .ThenInclude(b => b.Dhobi)
            .Include(o => o.RiderBids)
                .ThenInclude(rb => rb.Rider)
            .Include(o => o.Rider)
            .Include(o => o.Reviews)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order == null ? null : MapToResponse(order);
    }

    public async Task<bool> UpdateOrderStatusAsync(int orderId, UpdateStatusDto dto)
    {
        var nextStatus = dto.Status;
        var order = await _context.Orders
            .Include(o => o.Bids)
                .ThenInclude(b => b.Dhobi)
            .Include(o => o.SelectedBid)
            .Include(o => o.Customer)
            .Include(o => o.RiderBids)
            .FirstOrDefaultAsync(o => o.Id == orderId);
            
        if (order == null) return false;

        var currentStatus = order.Status;
        bool isValidTransition = false;

        if (currentStatus == OrderStatus.PickupScheduled && nextStatus == OrderStatus.PickedUp)
            isValidTransition = true;
        else if (currentStatus == OrderStatus.PickedUp && nextStatus == OrderStatus.InLaundry)
            isValidTransition = true;
        else if (currentStatus == OrderStatus.InLaundry && nextStatus == OrderStatus.ReadyForDelivery)
            isValidTransition = true;
        else if (currentStatus == OrderStatus.ReadyForDelivery && nextStatus == OrderStatus.OutForDelivery)
            isValidTransition = true;
        else if (currentStatus == OrderStatus.OutForDelivery && nextStatus == OrderStatus.Completed)
            isValidTransition = true;
        else if (nextStatus == OrderStatus.Cancelled)
            isValidTransition = true;

        if (!isValidTransition) return false;

        if ((nextStatus == OrderStatus.PickedUp || nextStatus == OrderStatus.OutForDelivery || nextStatus == OrderStatus.Completed) 
            && !order.RiderId.HasValue)
        {
            return false;
        }

        var payment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
        bool isWalletPayment = payment?.PaymentMethod == "Wallet";

        // --- RIDER PAYOUT (Leg 1: Pickup Complete) ---
        if (nextStatus == OrderStatus.InLaundry && order.RiderId.HasValue && isWalletPayment)
        {
            var deliveryFee = order.RiderFee ?? CalculateCurrentDeliveryFee(order);
            await ProcessWalletTransfer(order.CustomerId.GetValueOrDefault(), order.RiderId.Value, 
                deliveryFee, $"Pickup Delivery Fee #ORD-{order.Id}");
        }

        // --- DHOBI & RIDER PAYOUT (Leg 2: Service Completion) ---
        if (nextStatus == OrderStatus.Completed)
        {
            var bid = order.Bids.FirstOrDefault(b => b.Id == order.SelectedBidId);
            if (bid != null)
            {
                if (isWalletPayment)
                {
                    // Pay Dhobi
                    await ProcessWalletTransfer(order.CustomerId.GetValueOrDefault(), bid.DhobiId.GetValueOrDefault(), 
                        bid.Price, $"Laundry Service Payment #ORD-{order.Id}");

                    // Pay Rider
                    if (order.RiderId.HasValue)
                    {
                        var deliveryFee = order.RiderFee ?? CalculateCurrentDeliveryFee(order);
                        await ProcessWalletTransfer(order.CustomerId.GetValueOrDefault(), order.RiderId.Value, 
                            deliveryFee, $"Return Delivery Fee #ORD-{order.Id}");
                    }
                }
                else 
                {
                    // COD Settlement: Rider has physical cash for [Laundry + Delivery]
                    if (order.RiderId.HasValue)
                    {
                        // 1. Move Laundry Fee from Rider to Dhobi
                        await ProcessWalletTransfer(order.RiderId.Value, bid.DhobiId.GetValueOrDefault(), 
                            bid.Price, $"COD Settlement: Dhobi Laundry Fee #ORD-{order.Id}");

                        // 2. Note: Rider keeps their Delivery Fee from the cash. 
                        // The ProcessWalletTransfer above automatically handles the 7% Platform Commission 
                        // by taking it from the 'From' account (Rider) and giving to Admin.
                        
                        // 3. We also need to take commission on the DELIVERY fee itself from the Rider
                        var deliveryFee = order.RiderFee ?? CalculateCurrentDeliveryFee(order);
                        await ProcessWalletTransfer(order.RiderId.Value, 1, // 1 is Admin
                            deliveryFee, $"Commission on COD Delivery Fee #ORD-{order.Id}");
                    }
                }

                if (payment != null) {
                    payment.Status = PaymentStatus.Completed;
                } else {
                    _context.Payments.Add(new Payment {
                        OrderId = order.Id,
                        Amount = bid.Price,
                        PaymentMethod = "CashOnDelivery",
                        Status = PaymentStatus.Completed,
                        TransactionId = Guid.NewGuid().ToString("N").ToUpper(),
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        if (nextStatus == OrderStatus.InLaundry || nextStatus == OrderStatus.ReadyForDelivery)
        {
            order.RiderId = null; 
            if (nextStatus == OrderStatus.ReadyForDelivery)
            {
                // FORCE RESET: Remove all previous logistics bids to allow re-listing
                var oldRiderBids = await _context.RiderBids.Where(rb => rb.OrderId == order.Id).ToListAsync();
                if (oldRiderBids.Any())
                {
                    _context.RiderBids.RemoveRange(oldRiderBids);
                }
            }
        }

        order.Status = nextStatus;
        if (!string.IsNullOrEmpty(dto.ProofImageUrl))
        {
            order.ProofImageUrl = dto.ProofImageUrl; // Keep for legacy
            
            if (nextStatus == OrderStatus.PickedUp) order.PickupProofUrl = dto.ProofImageUrl;
            else if (nextStatus == OrderStatus.InLaundry) order.DhobiDropProofUrl = dto.ProofImageUrl;
            else if (nextStatus == OrderStatus.ReadyForDelivery) order.WashProofUrl = dto.ProofImageUrl; // Dhobi finishes wash
            else if (nextStatus == OrderStatus.OutForDelivery) order.DhobiPickupProofUrl = dto.ProofImageUrl; // Rider picks from shop
            else if (nextStatus == OrderStatus.Completed) order.DeliveryProofUrl = dto.ProofImageUrl;
        }

        await _context.SaveChangesAsync();
        await _notificationService.NotifyOrderUpdate(orderId, nextStatus.ToString());
        return true;
    }

    public async Task<bool> PlaceBidAsync(BidCreateDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Bids)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId);
            
        if (order == null) throw new Exception("Blueprint of order not found in registry.");
        if (order.Status != OrderStatus.PendingBidding) 
            throw new InvalidOperationException("Bidding manifest is closed for this order.");

        if (order.Bids.Any(b => b.DhobiId == dto.DhobiId))
            throw new InvalidOperationException("Identity matching: You have already committed a bid to this order.");

        var bid = new Bid
        {
            OrderId = dto.OrderId,
            DhobiId = dto.DhobiId,
            Price = dto.Price,
            DeliveryDays = dto.DeliveryDays,
            CreatedAt = DateTime.UtcNow
        };

        _context.Bids.Add(bid);
        await _context.SaveChangesAsync();

        // Real-time: Notify the customer that a new bid has arrived
        await _notificationService.NotifyNewBid(dto.OrderId, order.CustomerId.GetValueOrDefault());

        return true;
    }

    public async Task<IEnumerable<BidResponseDto>> GetBidsForOrderAsync(int orderId)
    {
        var bids = await _context.Bids
            .Include(b => b.Dhobi)
            .Where(b => b.OrderId == orderId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
            
        return bids.Select(b => new BidResponseDto
        {
            Id = b.Id,
            DhobiId = b.DhobiId.GetValueOrDefault(),
            DhobiName = b.Dhobi?.FullName ?? "Unknown",
            Price = b.Price,
            DeliveryDays = b.DeliveryDays,
            IsSelected = b.IsSelected,
            CreatedAt = b.CreatedAt,
            IsPremium = b.Dhobi?.SubscriptionExpiryDate.HasValue == true && b.Dhobi.SubscriptionExpiryDate > DateTime.UtcNow,
            DhobiRating = b.Dhobi?.Rating ?? 5.0,
            DhobiReviewsCount = b.Dhobi?.ReviewsCount ?? 0,
            HasOwnRider = b.Dhobi?.RiderType == RiderType.Linked || (b.Dhobi?.Role == UserRole.Dhobi && b.Dhobi.LinkedDhobiId != null) // Logic for Shop-linked riders
        });
    }

    public async Task<bool> SelectBidAsync(int orderId, int bidId)
    {
        var order = await _context.Orders
            .Include(o => o.Bids)
            .FirstOrDefaultAsync(o => o.Id == orderId);
            
        if (order == null) return false;

        var bid = await _context.Bids.FirstOrDefaultAsync(b => b.Id == bidId);
        if (bid == null) return false;

        // TIER CONCURRENCY LIMITS (University Proposal Spec)
        var dhobi = await _context.Users.FindAsync(bid.DhobiId);
        if (dhobi != null)
        {
            var activeOrdersCount = await _context.Orders
                .CountAsync(o => o.SelectedBid.DhobiId == dhobi.Id && 
                                 o.Status != OrderStatus.Completed && 
                                 o.Status != OrderStatus.Cancelled);
            
            int limit = 15; // Normal
            if (dhobi.DhobiType == DhobiType.FullTime) limit = 100;
            else if (dhobi.DhobiType == DhobiType.Premium) limit = 500;

            if (activeOrdersCount >= limit)
            {
                throw new InvalidOperationException($"Dhobi Tier Limit Reached: This partner can only handle {limit} concurrent orders.");
            }
        }

        bid.IsSelected = true;
        order.SelectedBidId = bidId;
        order.Status = OrderStatus.BidSelected;

        // FINANCIAL SETTLEMENT: Pre-pay laundry fees (Minus Platform Commmission)
        if (bid.DhobiId.HasValue)
        {
            await ProcessWalletTransfer(order.CustomerId.GetValueOrDefault(), bid.DhobiId.Value, bid.Price, $"Laundry Fees for Manifest #{orderId}");
            
            // INSURANCE COLLECTION: Rs. 30 goes directly to Platform Admin (User ID: 1)
            if (order.IsInsured && order.InsuranceFee > 0)
            {
                await ProcessWalletTransfer(order.CustomerId.GetValueOrDefault(), 1, order.InsuranceFee, $"Insurance Fee for Manifest #{orderId}");
            }
        }
        
        _context.Bids.Update(bid);
        _context.Orders.Update(order);

        await _context.SaveChangesAsync();
        await _notificationService.NotifyOrderUpdate(orderId, "BidSelected");
        return true;
    }

    public async Task<IEnumerable<OrderResponseDto>> GetDhobiOrdersAsync(int dhobiId)
    {
        var orders = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Bids)
            .Where(o => o.Bids.Any(b => b.DhobiId == dhobiId && b.IsSelected))
            .ToListAsync();

        return orders.Select(MapToResponse);
    }

    public async Task<IEnumerable<OrderResponseDto>> GetAvailableRiderTasksAsync()
    {
        var availableOrders = await _context.Orders
            .Include(o => o.Bids)
                .ThenInclude(b => b.Dhobi)
            .Include(o => o.RiderBids)
            .Include(o => o.Customer)
            .Where(o => o.RiderId == null && 
                       (o.Status == OrderStatus.BidSelected || o.Status == OrderStatus.ReadyForDelivery))
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();

        return availableOrders.Select(MapToResponse);
    }

    public async Task<bool> AcceptTaskAsync(int orderId, int riderId)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderId && o.RiderId == null);
        if (order == null) return false;

        order.RiderId = riderId;
        
        if (order.Status == OrderStatus.BidSelected) 
            order.Status = OrderStatus.PickupScheduled;
        else if (order.Status == OrderStatus.ReadyForDelivery)
            order.Status = OrderStatus.OutForDelivery;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelOrderAsync(int id)
    {
        var order = await _context.Orders.FindAsync(id);
        if (order == null) return false;
        if (order.Status != OrderStatus.PendingBidding && order.Status != OrderStatus.BidSelected) return false;

        order.Status = OrderStatus.Cancelled;
        await _context.SaveChangesAsync();
        return true;
    }

    public OrderResponseDto MapToResponse(Order order)
    {
        // Try all possible ways to find the selected bid
        var selectedBid = order.SelectedBid 
            ?? order.Bids.FirstOrDefault(b => b.IsSelected) 
            ?? order.Bids.FirstOrDefault(b => b.Id == (order.SelectedBidId ?? -1));

        double distance = 0;
        
        if (order.Latitude.HasValue && order.Latitude.Value != 0 && 
            selectedBid?.Dhobi != null && selectedBid.Dhobi.Latitude != 0)
        {
            distance = CalculateDistance(
                order.Latitude.Value, order.Longitude.Value, 
                selectedBid.Dhobi.Latitude, selectedBid.Dhobi.Longitude);
        }
        else 
        {
            // Simulation Fallback: Generate a stable distance based on ID for high-fidelity testing
            var seededRandom = new Random(order.Id + 100);
            distance = 1.8 + seededRandom.NextDouble() * 6.2;
        }

        var resFee = order.RiderFee ?? (decimal)Math.Max(100, Math.Round(distance * 50, 2));

        // Robust Dhobi Name Resolution
        var dhobiName = selectedBid?.Dhobi?.ShopName ?? selectedBid?.Dhobi?.FullName;
        
        // If we still don't have a name, scan the Bids collection manually
        if (string.IsNullOrEmpty(dhobiName))
        {
            var matchedBid = order.Bids.FirstOrDefault(b => b.IsSelected || (order.SelectedBidId.HasValue && b.Id == order.SelectedBidId));
            dhobiName = matchedBid?.Dhobi?.ShopName ?? matchedBid?.Dhobi?.FullName ?? matchedBid?.Dhobi?.Email;
        }

        return new OrderResponseDto
        {
            Id = order.Id,
            ServiceDescription = order.ServiceDescription,
            ItemsCount = order.ItemsCount,
            PickupAddress = order.PickupAddress,
            Status = order.Status.ToString(),
            BidCount = order.Bids.Count,
            DhobiName = string.IsNullOrEmpty(dhobiName) ? "Dhobi Partner" : dhobiName,
            DhobiAddress = selectedBid?.Dhobi?.Address ?? "No address set",
            CustomerName = order.Customer?.FullName ?? "Guest Customer",
            DistanceKm = Math.Round(distance, 2),
            SelectedBidPrice = selectedBid?.Price ?? 0,
            DeliveryFee = resFee,
            DhobiRating = selectedBid?.Dhobi?.Rating ?? 5.0,
            DhobiReviewsCount = selectedBid?.Dhobi?.ReviewsCount ?? 0,
            IsInsured = order.IsInsured,
            InsuranceFee = order.InsuranceFee,
            TotalAmount = (selectedBid?.Price ?? 0) + resFee + order.InsuranceFee,
            IsReviewed = order.Reviews.Any(),
            CreatedAt = order.CreatedAt,
            ProofImageUrl = order.ProofImageUrl,
            PickupProofUrl = order.PickupProofUrl,
            DhobiDropProofUrl = order.DhobiDropProofUrl,
            WashProofUrl = order.WashProofUrl,
            DhobiPickupProofUrl = order.DhobiPickupProofUrl,
            DeliveryProofUrl = order.DeliveryProofUrl,
            ClothImageUrl = order.ClothImageUrl,
            RiderLatitude = order.Rider?.Latitude,
            RiderLongitude = order.Rider?.Longitude,
            Bids = order.Bids.Select(b => new BidResponseDto
            {
                Id = b.Id,
                DhobiId = b.DhobiId.GetValueOrDefault(),
                DhobiName = b.Dhobi?.FullName ?? "Unknown",
                Price = b.Price,
                DeliveryDays = b.DeliveryDays,
                IsSelected = b.IsSelected,
                DhobiRating = b.Dhobi?.Rating ?? 5.0,
                DhobiReviewsCount = b.Dhobi?.ReviewsCount ?? 0,
                CreatedAt = b.CreatedAt,
                IsPremium = b.Dhobi?.DhobiType == DhoobiGO.Domain.Enums.DhobiType.Premium,
                HasOwnRider = b.Dhobi != null && _context.Users.Any(u => u.LinkedDhobiId == b.DhobiId && u.IsLinkVerified)
            }).ToList(),
            RiderBids = order.RiderBids.Select(rb => new RiderBidResponseDto
            {
                Id = rb.Id,
                RiderId = rb.RiderId,
                RiderName = rb.Rider?.FullName ?? "Rider",
                OfferedFee = rb.OfferedFee,
                RiderRating = rb.Rider?.Rating ?? 5.0,
                RiderReviewsCount = rb.Rider?.ReviewsCount ?? 0,
                IsAccepted = rb.IsAccepted,
                CreatedAt = rb.CreatedAt
            }).ToList()
        };
    }

    private decimal CalculateCurrentDeliveryFee(Order order)
    {
        var selectedBid = order.Bids.FirstOrDefault(b => b.Id == order.SelectedBidId);
        double distance = 0;
        
        if (order.Latitude.HasValue && order.Longitude.HasValue && 
            selectedBid?.Dhobi != null && selectedBid.Dhobi.Latitude != 0)
        {
            distance = CalculateDistance(
                order.Latitude.Value, order.Longitude.Value,
                selectedBid.Dhobi.Latitude, selectedBid.Dhobi.Longitude);
        }

        return (decimal)Math.Max(100, Math.Round(distance * 50, 2));
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;          
    }

    private double ToRadians(double deg) => deg * (Math.PI / 180);

    private async Task ProcessWalletTransfer(int fromUserId, int toUserId, decimal totalAmount, string description)
    {
        var fromWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == fromUserId);
        var toWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == toUserId);
        var adminWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == 1); // System Admin

        if (fromWallet == null || toWallet == null || adminWallet == null) return;

        // --- ACCURACY CHECK: Insufficient Funds ---
        if (fromWallet.Balance < totalAmount && fromUserId != 1) // Admin can go negative for system adjustments
        {
            throw new InvalidOperationException($"Insufficient Wallet Balance. This manifest requires Rs. {totalAmount}, but your current balance is Rs. {fromWallet.Balance}.");
        }

        // PLATFORM MONETIZATION: 7% Standard Commission (University Proposal Spec)
        decimal commissionRate = 0.07m;
        
        // Premium partners (Dhobi/Rider) pay 0% commission on orders if they have an ACTIVE subscription
        var recipient = await _context.Users.FindAsync(toUserId);
        if (toUserId == 1 || (recipient != null && recipient.SubscriptionExpiryDate.HasValue && recipient.SubscriptionExpiryDate > DateTime.UtcNow))
        {
            commissionRate = 0;
        }

        decimal commission = totalAmount * commissionRate;
        decimal netAmount = totalAmount - commission;

        fromWallet.Balance -= totalAmount;
        toWallet.Balance += netAmount;
        adminWallet.Balance += commission;

        fromWallet.Transactions.Add(new WalletTransaction { 
            Amount = totalAmount, Description = description, Type = "Debit", Timestamp = DateTime.UtcNow 
        });
        toWallet.Transactions.Add(new WalletTransaction { 
            Amount = netAmount, Description = $"{description} (Net)", Type = "Credit", Timestamp = DateTime.UtcNow 
        });
        
        if (commission > 0)
        {
            adminWallet.Transactions.Add(new WalletTransaction { 
                Amount = commission, Description = $"Commission from: {description}", Type = "Credit", Timestamp = DateTime.UtcNow 
            });
        }
    }

    public async Task<bool> PlaceRiderBidAsync(int orderId, int riderId, decimal fee)
    {
        var order = await _context.Orders
            .Include(o => o.RiderBids)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) throw new Exception("Order logistics target not found.");
        
        // Only allow bidding if status is BidSelected or ReadyForDelivery
        if (order.Status != OrderStatus.BidSelected && order.Status != OrderStatus.ReadyForDelivery)
            throw new InvalidOperationException("Logistics bid portal is currently locked for this order.");

        var existingBid = order.RiderBids.FirstOrDefault(rb => rb.RiderId == riderId);
        if (existingBid != null)
        {
            if (order.Status == OrderStatus.ReadyForDelivery)
            {
                // Self-healing: Cleanup stale Stage 1 bid if still present during Stage 2
                _context.RiderBids.Remove(existingBid);
                await _context.SaveChangesAsync();
            }
            else 
            {
                throw new InvalidOperationException("Logistics conflict: You have already submitted a delivery bid.");
            }
        }

        var bid = new RiderBid
        {
            OrderId = orderId,
            RiderId = riderId,
            OfferedFee = fee,
            CreatedAt = DateTime.UtcNow
        };

        _context.RiderBids.Add(bid);
        await _context.SaveChangesAsync();
        await _notificationService.NotifyOrderUpdate(orderId, "NewRiderBid");
        return true;
    }

    public async Task<bool> AcceptRiderBidAsync(int orderId, int bidId)
    {
        var bid = await _context.RiderBids
            .Include(b => b.Order)
            .FirstOrDefaultAsync(b => b.Id == bidId && b.OrderId == orderId);

        if (bid == null || bid.Order == null) return false;

        var others = await _context.RiderBids.Where(b => b.OrderId == orderId).ToListAsync();
        foreach (var b in others) b.IsAccepted = false;

        bid.IsAccepted = true;
        bid.Order.RiderId = bid.RiderId;
        bid.Order.RiderFee = bid.OfferedFee;

        // FINANCIAL SETTLEMENT: Deduct delivery fee from customer
        string feeType = bid.Order.Status == OrderStatus.ReadyForDelivery ? "Return Delivery" : "Pickup";
        await ProcessWalletTransfer(bid.Order.CustomerId.GetValueOrDefault(), bid.RiderId, bid.OfferedFee, $"{feeType} Fee for Manifest #{orderId}");

        if (bid.Order.Status == OrderStatus.PendingBidding || bid.Order.Status == OrderStatus.BidSelected)
        {
            bid.Order.Status = OrderStatus.PickupScheduled;
        }
        else if (bid.Order.Status == OrderStatus.ReadyForDelivery)
        {
            bid.Order.Status = OrderStatus.OutForDelivery;
        }
        
        await _context.SaveChangesAsync();
        await _notificationService.NotifyOrderUpdate(orderId, "RiderAccepted");
        return true;
    }

    public async Task<bool> RaiseDisputeAsync(int orderId, string reason)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order == null) return false;

        // Transition to Disputed state
        order.Status = OrderStatus.Disputed;

        // Automated Chat System: Notify Admin (User ID: 1)
        var disputeMessage = new ChatMessage
        {
            GroupName = orderId.ToString(),
            SenderName = order.Customer?.FullName ?? "Customer",
            Message = $"[DISPUTE RAISED] Order #{orderId}: {reason}",
            Timestamp = DateTime.UtcNow
        };

        _context.ChatMessages.Add(disputeMessage);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> BatchAssignRiderAsync(List<int> orderIds, int riderId)
    {
        var orders = await _context.Orders
            .Where(o => orderIds.Contains(o.Id))
            .ToListAsync();

        if (orders.Count == 0) return false;

        foreach (var order in orders)
        {
            order.RiderId = riderId;
            if (order.Status == OrderStatus.BidSelected)
                order.Status = OrderStatus.PickupScheduled;
            else if (order.Status == OrderStatus.ReadyForDelivery)
                order.Status = OrderStatus.OutForDelivery;
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
