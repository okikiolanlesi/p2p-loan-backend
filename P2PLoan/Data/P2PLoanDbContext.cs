using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using P2PLoan.Models;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace P2PLoan.Data;

public class P2PLoanDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public P2PLoanDbContext(DbContextOptions options, IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // modelBuilder.Entity<User>()
        //     .Property(e => e.Role)
        //     .HasConversion<string>(); // Convert enum to string

        base.OnModelCreating(modelBuilder);

        ConfigureAuditableEntity<Module>(modelBuilder);
        ConfigureAuditableEntity<Permission>(modelBuilder);
        ConfigureAuditableEntity<Role>(modelBuilder);
        ConfigureAuditableEntity<UserRole>(modelBuilder);
        ConfigureAuditableEntity<LoanOffer>(modelBuilder);
        ConfigureAuditableEntity<Repayment>(modelBuilder);
        ConfigureAuditableEntity<Transaction>(modelBuilder);

        // Configure the relationship between LoanOffer and User
            modelBuilder.Entity<LoanOffer>()
                .HasOne(offer => offer.CreatedBy)
                .WithMany(user => user.LoanOffers)
                .HasForeignKey(offer => offer.CreatedById)
                .OnDelete(DeleteBehavior.Restrict); 

            modelBuilder.Entity<Repayment>()
        .HasOne(r => r.Loan)
        .WithMany()
        //.HasForeignKey(r => r.LoanId)
        .OnDelete(DeleteBehavior.Restrict);

             // Configure Repayment entity
        // modelBuilder.Entity<Repayment>()
        //     .HasOne(r => r.User)
        //     .WithMany(u => u.Repayments)
        //     .HasForeignKey(r => r.UserId)
        //     .IsRequired();

        // Configure additional relationships if needed
    // modelBuilder.Entity<Repayment>()
    //     .HasOne(r => r.Loan)
    //     .WithMany()
    //     //.HasForeignKey(r => r.LoanId)
    //     .OnDelete(DeleteBehavior.Restrict);

    // modelBuilder.Entity<Repayment>()
    //     .HasOne<User>()
    //     .WithMany(u => u.Repayments)
    //     .HasForeignKey(r => r.UserId)
    //     .OnDelete(DeleteBehavior.Restrict);

           modelBuilder.Entity<Transaction>()
                .HasOne(transaction => transaction.CreatedBy)
                .WithMany(user => user.Transactions)
                .HasForeignKey(transaction => transaction.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure one-to-many relationship between User and Wallet
            modelBuilder.Entity<User>()
                .HasMany(u => u.Wallets)
                .WithOne(w => w.User)
                //.HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            
             
    }
    private void ConfigureAuditableEntity<TEntity>(ModelBuilder modelBuilder) where TEntity : AuditableEntity
    {
        modelBuilder.Entity<TEntity>()
            .HasOne(e => e.CreatedBy)
            .WithMany()
            .HasForeignKey(e => e.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TEntity>()
            .HasOne(e => e.ModifiedBy)
            .WithMany()
            .HasForeignKey(e => e.ModifiedById)
            .OnDelete(DeleteBehavior.Restrict);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Seed> Seeds { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
<<<<<<< HEAD
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanOffer> LoanOffers{ get; set; }
    public DbSet<Repayment> Repayments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionType> TransactionTypes{ get; set; }
    public DbSet<Wallet> Wallets{ get; set; }
    public DbSet<WalletProvider> WalletProviders{ get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);

    }

    private void UpdateAuditFields()
    {
        var userId = GetCurrentUserId();

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                entry.Entity.CreatedById = userId;
            }

            if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifiedAt = DateTime.UtcNow;
                entry.Entity.ModifiedById = userId;
            }
        }
        
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }

=======
    public DbSet<Notification> Notifications {get; set;}
>>>>>>> 2c53b5e461aeb12cc53fff588328757cceab5836
}

