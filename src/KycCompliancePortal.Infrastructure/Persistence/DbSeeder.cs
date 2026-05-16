using KycCompliancePortal.Core.Entities;
using KycCompliancePortal.Core.Enums;
using KycCompliancePortal.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace KycCompliancePortal.Infrastructure.Persistence;

/// <summary>
/// Creates the database and seeds default accounts + a sample watchlist so the
/// API is usable immediately after first run.
/// </summary>
public static class DbSeeder
{
    public const string DefaultTenant = "BANK001";

    public static async Task SeedAsync(AppDbContext db, IPasswordHasher hasher)
    {
        // Creates the SQLite schema from the model on first run — no EF tooling
        // required. In production this would be db.Database.MigrateAsync().
        await db.Database.EnsureCreatedAsync();

        if (!await db.Users.AnyAsync())
        {
            db.Users.AddRange(
                new User
                {
                    Email = "keval@bank001.com",
                    PasswordHash = hasher.Hash("Admin@123"),
                    Role = UserRole.Admin,
                    TenantId = DefaultTenant
                },
                new User
                {
                    Email = "hardik@bank001.com",
                    PasswordHash = hasher.Hash("Officer@123"),
                    Role = UserRole.ComplianceOfficer,
                    TenantId = DefaultTenant
                });
        }

        if (!await db.SanctionedEntities.AnyAsync())
        {
            db.SanctionedEntities.AddRange(
                new SanctionedEntity { FullName = "Ravi Mehta",    Country = "India",     ListSource = "OFAC" },
                new SanctionedEntity { FullName = "Keval Gelani",  Country = "India",     ListSource = "UN" },
                new SanctionedEntity { FullName = "Hardik Patel",  Country = "India",     ListSource = "UN" },
                new SanctionedEntity { FullName = "Kenil Shah",    Country = "UAE",       ListSource = "OFAC" },
                new SanctionedEntity { FullName = "Jay Trivedi",   Country = "Singapore", ListSource = "OFAC" });
        }

        await db.SaveChangesAsync();
    }
}
