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
        // modelBuilder.Entity<User>()
        //     .Property(e => e.Role)
        //     .HasConversion<string>(); // Convert enum to string

        base.OnModelCreating(modelBuilder);

        ConfigureAuditableEntity<Module>(modelBuilder);
        ConfigureAuditableEntity<Permission>(modelBuilder);
        ConfigureAuditableEntity<Role>(modelBuilder);
        ConfigureAuditableEntity<UserRole>(modelBuilder);
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
}

