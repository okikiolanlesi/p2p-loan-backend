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
        // Configure LoanRequest and User relationship
        modelBuilder.Entity<LoanRequest>()
            .HasOne(lr => lr.User)
            .WithMany(u => u.LoanRequests)
            .HasForeignKey(lr => lr.UserId)
            .OnDelete(DeleteBehavior.NoAction);

         // Configure the one-to-many relationship between User and Wallet
        modelBuilder.Entity<User>()
            .HasMany(u => u.Wallets)
            .WithOne(w => w.User)
            .HasForeignKey(w => w.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);

        // Configure precision and scale for the decimal properties in Loan
            modelBuilder.Entity<Loan>()
                .Property(l => l.AccruingInterestRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Loan>()
                .Property(l => l.InitialInterestRate)
                .HasPrecision(18, 2);

             modelBuilder.Entity<LoanOffer>()
                .Property(lo => lo.AccruingInterest)
                .HasPrecision(18, 2);

            // Configure precision and scale for the decimal properties in Repayment
            modelBuilder.Entity<Repayment>()
                .Property(r => r.InterestRate)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Repayment>()
                .Property(r => r.PrincipalAmount)
                .HasPrecision(18, 2);

        ConfigureAuditableEntity<Module>(modelBuilder);
        ConfigureAuditableEntity<Permission>(modelBuilder);
        ConfigureAuditableEntity<Role>(modelBuilder);
        ConfigureAuditableEntity<UserRole>(modelBuilder);
        ConfigureAuditableEntity<LoanOffer>(modelBuilder);
        ConfigureAuditableEntity<Repayment>(modelBuilder);
        ConfigureAuditableEntity<Transaction>(modelBuilder);  
        ConfigureAuditableEntity<Loan>(modelBuilder);  
        ConfigureAuditableEntity<Wallet>(modelBuilder); 
        ConfigureAuditableEntity<LoanRequest>(modelBuilder); 
         ConfigureAuditableEntity<TransactionType>(modelBuilder); 
        ConfigureAuditableEntity<WalletProvider>(modelBuilder);  
        ConfigureAuditableEntity<NotificationTemplateVariable>(modelBuilder);  
        ConfigureAuditableEntity<NotificationTemplate>(modelBuilder);            
             
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
    public DbSet<Loan> Loans { get; set; }
    public DbSet<LoanOffer> LoanOffers{ get; set; }
    public DbSet<LoanRequest> LoanRequests{ get; set; }
    public DbSet<Repayment> Repayments { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<TransactionType> TransactionTypes{ get; set; }
    public DbSet<Wallet> Wallets{ get; set; }
    public DbSet<WalletProvider> WalletProviders{ get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<NotificationTemplate> NotificationTemplates{ get; set; }
    public DbSet<NotificationTemplateVariable> NotificationTemplateVariables{ get; set; }

    // public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    // {
    //     UpdateAuditFields();
    //     return base.SaveChangesAsync(cancellationToken);

    // }

    // private void UpdateAuditFields()
    // {
    //     var userId = GetCurrentUserId();

    //     foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
    //     {
    //         if (entry.State == EntityState.Added)
    //         {
    //             entry.Entity.CreatedAt = DateTime.UtcNow;
    //             entry.Entity.CreatedById = userId;
    //         }

    //         if (entry.State == EntityState.Modified)
    //         {
    //             entry.Entity.ModifiedAt = DateTime.UtcNow;
    //             entry.Entity.ModifiedById = userId;
    //         }
    //     }
        
    // }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return userIdClaim != null ? Guid.Parse(userIdClaim.Value) : Guid.Empty;
    }

}

