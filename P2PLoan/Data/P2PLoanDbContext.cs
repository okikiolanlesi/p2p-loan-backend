using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Constants;
using P2PLoan.Models;

namespace P2PLoan.Data
{
    public class P2PLoanDbContext : DbContext
    {
        public P2PLoanDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the enum converter for MyEntity and its MyEnumProperty
            ConfigureEnumConverter<WalletProvider, WalletProviders>(modelBuilder, e => e.Slug);
            ConfigureEnumConverter<Permission, PermissionAction>(modelBuilder, e => e.Action);
            ConfigureEnumConverter<User, UserType>(modelBuilder, e => e.UserType);
            ConfigureEnumConverter<Module, Modules>(modelBuilder, e => e.Identifier);
            ConfigureEnumConverter<LoanOffer, LoanOfferType>(modelBuilder, e => e.Type);
            ConfigureEnumConverter<LoanOffer, PaymentFrequency>(modelBuilder, e => e.RepaymentFrequency);
            ConfigureEnumConverter<LoanRequest, LoanRequestStatus>(modelBuilder, e => e.Status);
            ConfigureEnumConverter<Loan, LoanStatus>(modelBuilder, e => e.Status);
            ConfigureEnumConverter<Loan, PaymentFrequency>(modelBuilder, e => e.RepaymentFrequency);

            // Configure unique properties on tables
            ConfigureUniqueProperty<Module>(modelBuilder, x => x.Identifier);
            ConfigureUniqueProperty<User>(modelBuilder, x => x.Email);
            ConfigureUniqueProperty<WalletProvider>(modelBuilder, x => x.Slug);

            // Configure auditable entities
            ConfigureAuditableEntity<Module>(modelBuilder);
            ConfigureAuditableEntity<Permission>(modelBuilder);
            ConfigureAuditableEntity<Role>(modelBuilder);
            ConfigureAuditableEntity<UserRole>(modelBuilder);
            ConfigureAuditableEntity<Wallet>(modelBuilder);
            ConfigureAuditableEntity<WalletProvider>(modelBuilder);
            ConfigureAuditableEntity<RolePermission>(modelBuilder);
            ConfigureAuditableEntity<LoanOffer>(modelBuilder);
            ConfigureAuditableEntity<LoanRequest>(modelBuilder);
            ConfigureAuditableEntity<Loan>(modelBuilder);

            // Configure delete behavior
            ConfigureDeleteBehavior<LoanOffer, Wallet>(modelBuilder, e => e.Wallet, DeleteBehavior.Restrict);
            ConfigureDeleteBehavior<LoanOffer, User>(modelBuilder, e => e.User, DeleteBehavior.Restrict);
            ConfigureDeleteBehavior<LoanRequest, Wallet>(modelBuilder, e => e.Wallet, DeleteBehavior.Restrict);
            ConfigureDeleteBehavior<LoanRequest, LoanOffer>(modelBuilder, e => e.LoanOffer, DeleteBehavior.Restrict);
            ConfigureDeleteBehavior<LoanRequest, User>(modelBuilder, e => e.User, DeleteBehavior.Restrict);
            ConfigureDeleteBehavior<Loan, User>(modelBuilder, e => e.Borrower, DeleteBehavior.Restrict);
            ConfigureDeleteBehavior<Loan, User>(modelBuilder, e => e.Lender, DeleteBehavior.Restrict);
            ConfigureDeleteBehavior<Loan, LoanOffer>(modelBuilder, e => e.LoanOffer, DeleteBehavior.Restrict);
            ConfigureDeleteBehavior<Loan, LoanRequest>(modelBuilder, e => e.LoanRequest, DeleteBehavior.Restrict);
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

        private void ConfigureEnumConverter<TEntity, TProperty>(ModelBuilder modelBuilder, Expression<Func<TEntity, TProperty>> propertyExpression) where TEntity : class
        {
            modelBuilder.Entity<TEntity>()
                .Property(propertyExpression)
                .HasConversion<string>(); // Convert enum to string
        }

        private void ConfigureUniqueProperty<TEntity>(ModelBuilder modelBuilder, Expression<Func<TEntity, object>> propertyExpression) where TEntity : class
        {
            modelBuilder.Entity<TEntity>()
                .HasIndex(propertyExpression)
                .IsUnique();
        }

        private void ConfigureDeleteBehavior<TEntity, TRelatedEntity>(ModelBuilder modelBuilder, Expression<Func<TEntity, TRelatedEntity>> navigationExpression, DeleteBehavior deleteBehavior)
            where TEntity : class
            where TRelatedEntity : class
        {
            modelBuilder.Entity<TEntity>()
                .HasOne(navigationExpression)
                .WithMany()
                .OnDelete(deleteBehavior);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Module> Modules { get; set; }
        public DbSet<Seed> Seeds { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletProvider> WalletProviders { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<LoanOffer> LoanOffers { get; set; }
        public DbSet<LoanRequest> LoanRequests { get; set; }
        public DbSet<Loan> Loans { get; set; }

    }
}
