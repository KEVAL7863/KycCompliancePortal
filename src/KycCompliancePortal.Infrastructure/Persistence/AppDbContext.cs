using KycCompliancePortal.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace KycCompliancePortal.Infrastructure.Persistence;

/// <summary>
/// EF Core context. Configured for SQLite in development; switching to SQL
/// Server in production is just a provider + connection-string change in
/// Program.cs — the model below is provider-agnostic.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<KycDocument> KycDocuments => Set<KycDocument>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<SanctionedEntity> SanctionedEntities => Set<SanctionedEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => new { u.TenantId, u.Email }).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.TenantId).IsRequired().HasMaxLength(64);
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.Property(c => c.FullName).IsRequired().HasMaxLength(256);
            e.Property(c => c.AnnualIncome).HasPrecision(18, 2);
            e.Property(c => c.ExpectedMonthlyTransactionVolume).HasPrecision(18, 2);
            e.HasIndex(c => new { c.TenantId, c.Email });
            e.HasMany(c => c.Documents)
             .WithOne(d => d.Customer!)
             .HasForeignKey(d => d.CustomerId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AuditLog>(e =>
        {
            e.Property(a => a.Action).IsRequired().HasMaxLength(128);
            e.HasIndex(a => a.TenantId);
            e.HasIndex(a => a.TimestampUtc);
        });

        modelBuilder.Entity<SanctionedEntity>(e =>
        {
            e.Property(s => s.FullName).IsRequired().HasMaxLength(256);
            e.HasIndex(s => s.FullName);
        });

        base.OnModelCreating(modelBuilder);
    }
}
