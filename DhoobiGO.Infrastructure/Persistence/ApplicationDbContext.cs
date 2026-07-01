using DhoobiGO.Application.Interfaces;
using DhoobiGO.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DhoobiGO.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IConfiguration _configuration;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration) : base(options)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Bid> Bids => Set<Bid>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserAddress> UserAddresses { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<PaymentMethod> PaymentMethods { get; set; }
    public DbSet<DhobiService> DhobiServices => Set<DhobiService>();
    public DbSet<RiderBid> RiderBids => Set<RiderBid>();
    public virtual DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- UserAddress Configuration ---
        modelBuilder.Entity<UserAddress>()
            .HasOne(ua => ua.User)
            .WithMany(u => u.SavedAddresses)
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Notification Configuration ---
        modelBuilder.Entity<Notification>()
            .HasOne(n => n.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // --- Global Query Filters ---
        modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);

        // Configure Relationships
        
        // Order - Customer (User)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.ClientSetNull); // Allow order to exist if user is filtered

        // Order - SelectedBid
        modelBuilder.Entity<Order>()
            .HasOne(o => o.SelectedBid)
            .WithOne()
            .HasForeignKey<Order>(o => o.SelectedBidId)
            .OnDelete(DeleteBehavior.SetNull);

        // Bid - Order
        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Order)
            .WithMany(o => o.Bids)
            .HasForeignKey(b => b.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Bid - Dhobi (User)
        modelBuilder.Entity<Bid>()
            .HasOne(b => b.Dhobi)
            .WithMany()
            .HasForeignKey(b => b.DhobiId)
            .OnDelete(DeleteBehavior.ClientSetNull);
            
        // Payment - Order
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Order)
            .WithOne()
            .HasForeignKey<Payment>(p => p.OrderId);

        // Review - Order (One order can have multiple reviews: dhobi & rider)
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Order)
            .WithMany(o => o.Reviews)
            .HasForeignKey(r => r.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Review - Customer
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Customer)
            .WithMany()
            .HasForeignKey(r => r.CustomerId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        // Review - Dhobi
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Dhobi)
            .WithMany(u => u.ReviewsAsDhobi)
            .HasForeignKey(r => r.DhobiId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        // Review - Rider
        modelBuilder.Entity<Review>()
            .HasOne(r => r.Rider)
            .WithMany(u => u.ReviewsAsRider)
            .HasForeignKey(r => r.RiderId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        // --- Seed Default Admin from Config ---
        var adminSettings = _configuration.GetSection("AdminSettings");
        var adminEmail = adminSettings["Email"] ?? "admin@dhoobi.com";
        var adminPassword = adminSettings["Password"] ?? "Admin@123";
        var adminName = adminSettings["FullName"] ?? "System Administrator";

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            FullName = adminName,
            Email = adminEmail,
            PasswordHash = "$2a$11$Cx9So5ki0HB4olnZybRtU.Ke/SZUPes2ZyVVc9G2sNgelYbJo1XVW", // Static hash for Admin@123
            PhoneNumber = "0000000000",
            Address = "Platform HQ",
            Role = DhoobiGO.Domain.Enums.UserRole.Admin,
            IsVerified = true,
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });

        // --- Seed Platform Standard Services ---
        modelBuilder.Entity<ServiceType>().HasData(
            new ServiceType { Id = 101, Name = "Washing", Description = "Professional machine wash and tumble dry", Icon = "water", BasePrice = 50, Category = "Cleaning" },
            new ServiceType { Id = 102, Name = "Ironing", Description = "Steam iron and hangers/folding", Icon = "flash", BasePrice = 20, Category = "Ironing" },
            new ServiceType { Id = 103, Name = "Dry Clean", Description = "Delicate chemical cleaning for premium wear", Icon = "star", BasePrice = 300, Category = "Special" },
            new ServiceType { Id = 104, Name = "Heavy Duty", Description = "Blankets, curtains and heavy fabrics", Icon = "bed", BasePrice = 500, Category = "Cleaning" },
            new ServiceType { Id = 105, Name = "Steam Press", Description = "Premium steam pressing for suits and dresses", Icon = "shirt", BasePrice = 100, Category = "Ironing" }
        );
    }
}
