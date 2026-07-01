using DhoobiGO.Application.DTOs;
using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using DhoobiGO.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace DhoobiGO.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IApplicationDbContext _context;

    public PaymentService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> ProcessPaymentAsync(PaymentRequestDto dto)
    {
        var order = await _context.Orders
            .Include(o => o.Customer)
            .Include(o => o.Bids)
                .ThenInclude(b => b.Dhobi)
            .FirstOrDefaultAsync(o => o.Id == dto.OrderId);
            
        if (order == null) return false;

        // --- Wallet Logic ---
        if (dto.PaymentMethod == "Wallet")
        {
            var customerId = order.CustomerId ?? 0;
            var customerWallet = await _context.Wallets.FirstOrDefaultAsync(w => w.UserId == customerId);
            
            if (customerWallet == null || customerWallet.Balance < dto.Amount)
                return false; // Insufficient funds

            // 1. Deduct from Customer
            await AdjustBalanceAsync(customerId, -dto.Amount, $"Payment for Order #{order.Id}");

            // 2. Credit to Dhobi (Total Amount)
            var bid = order.Bids.FirstOrDefault(b => b.Id == order.SelectedBidId);
            if (bid != null && bid.DhobiId.HasValue)
            {
                await AdjustBalanceAsync(bid.DhobiId.Value, dto.Amount, $"Earnings from Order #{order.Id}");

                // 3. Handle Platform Commission (Revenue)
                var dhobi = bid.Dhobi;
                if (dhobi != null && dhobi.DhobiType != DhobiType.Premium)
                {
                    // Deduct 10% commission from Dhobi's newly received funds
                    var commission = dto.Amount * 0.1m;
                    await AdjustBalanceAsync(bid.DhobiId.Value, -commission, $"Platform Commission (10%) for Order #{order.Id}");
                }
            }
        }

        var payment = new Payment
        {
            OrderId = dto.OrderId,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            TransactionId = Guid.NewGuid().ToString("N").ToUpper(),
            Status = PaymentStatus.Completed
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<object?> GetPaymentStatusAsync(int orderId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.OrderId == orderId);
            
        if (payment == null) return null;

        return new
        {
            payment.TransactionId,
            payment.Amount,
            payment.PaymentMethod,
            Status = payment.Status.ToString(),
            payment.CreatedAt
        };
    }

    public async Task<decimal> GetWalletBalanceAsync(int userId)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);
            
        return wallet?.Balance ?? 0;
    }

    public async Task<IEnumerable<object>> GetSavedCardsAsync(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.PaymentMethods)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null) return Enumerable.Empty<object>();

        return wallet.PaymentMethods.Select(m => new
        {
            m.Id,
            m.Brand,
            m.Last4,
            m.ExpiryDate,
            m.IsDefault
        });
    }

    public async Task<bool> AddPaymentMethodAsync(int userId, AddPaymentMethodDto dto)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
        {
            wallet = new Wallet { UserId = userId, Balance = 0 };
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        var method = new PaymentMethod
        {
            WalletId = wallet.Id,
            Brand = dto.Brand,
            Last4 = dto.Last4,
            ExpiryDate = dto.ExpiryDate,
            IsDefault = !wallet.PaymentMethods.Any()
        };

        _context.PaymentMethods.Add(method);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AdjustBalanceAsync(int userId, decimal amount, string description)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
        {
            wallet = new Wallet { UserId = userId, Balance = 0 };
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync(); // Get the ID
        }

        wallet.Balance += amount;
        wallet.Transactions.Add(new WalletTransaction
        {
            Amount = Math.Abs(amount),
            Description = description,
            Type = amount >= 0 ? "Credit" : "Debit",
            Timestamp = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CreateWithdrawalRequestAsync(int userId, decimal amount, string method)
    {
        var wallet = await _context.Wallets
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null || wallet.Balance < amount)
            return false;

        // In a real app, we would create a PayoutRequest record. 
        // For DhoobiGO, we deduct directly and log the transaction.
        return await AdjustBalanceAsync(userId, -amount, $"Withdrawal via {method}");
    }

    public async Task<object?> GetWalletAsync(int userId)
    {
        var wallet = await _context.Wallets
            .Include(w => w.Transactions)
            .FirstOrDefaultAsync(w => w.UserId == userId);

        if (wallet == null)
        {
            wallet = new Wallet { UserId = userId, Balance = 0 };
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        var transactions = wallet.Transactions
            .OrderByDescending(t => t.Timestamp)
            .ToList();

        return new
        {
            Balance = wallet.Balance,
            Pending = 0, // In future, sum up pending settlement requests
            CompletedCount = transactions.Count(t => t.Type == "Credit"),
            Transactions = transactions.Select(t => new {
                t.Id,
                t.Amount,
                t.Description,
                Type = t.Type,
                t.Timestamp
            })
        };
    }
}
