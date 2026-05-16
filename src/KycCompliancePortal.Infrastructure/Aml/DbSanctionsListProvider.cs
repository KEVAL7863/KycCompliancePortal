using KycCompliancePortal.Core.Entities;
using KycCompliancePortal.Core.Interfaces;
using KycCompliancePortal.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KycCompliancePortal.Infrastructure.Aml;

/// <summary>
/// Production sanctions list backed by the database. The list is small and
/// changes rarely, so it is cached in memory for the lifetime of the request
/// scope rather than re-queried per name.
/// </summary>
public class DbSanctionsListProvider : ISanctionsListProvider
{
    private readonly AppDbContext _db;
    private IReadOnlyCollection<SanctionedEntity>? _cache;

    public DbSanctionsListProvider(AppDbContext db) => _db = db;

    public IReadOnlyCollection<SanctionedEntity> GetAll()
        => _cache ??= _db.SanctionedEntities.AsNoTracking().ToList();
}
