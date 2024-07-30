using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Models;

namespace P2PLoan.Data;

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

        ConfigureAuditableEntity<Module>(modelBuilder);
        ConfigureAuditableEntity<Permission>(modelBuilder);
        ConfigureAuditableEntity<Role>(modelBuilder);
        ConfigureAuditableEntity<UserRole>(modelBuilder);
        ConfigureAuditableEntity<Wallet>(modelBuilder);
        ConfigureAuditableEntity<WalletProvider>(modelBuilder);
        ConfigureAuditableEntity<RolePermission>(modelBuilder);
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

    public DbSet<User> Users { get; set; }
    public DbSet<Module> Modules { get; set; }
    public DbSet<Seed> Seeds { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<WalletProvider> WalletProviders { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
}

