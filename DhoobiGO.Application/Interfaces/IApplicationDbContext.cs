using DhoobiGO.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DhoobiGO.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }
    DbSet<Order> Orders { get; }
    DbSet<Bid> Bids { get; }
    DbSet<ServiceType> ServiceTypes { get; }
    DbSet<Payment> Payments { get; }
    DbSet<ChatMessage> ChatMessages { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Wallet> Wallets { get; }
    DbSet<WalletTransaction> WalletTransactions { get; }
    DbSet<UserAddress> UserAddresses { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<PaymentMethod> PaymentMethods { get; }
    DbSet<DhobiService> DhobiServices { get; }
    DbSet<RiderBid> RiderBids { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
