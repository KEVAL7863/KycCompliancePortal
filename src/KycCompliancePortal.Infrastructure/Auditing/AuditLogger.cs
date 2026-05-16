using KycCompliancePortal.Core.Entities;
using KycCompliancePortal.Core.Interfaces;
using KycCompliancePortal.Infrastructure.Persistence;

namespace KycCompliancePortal.Infrastructure.Auditing;

/// <summary>Appends an immutable row to the audit trail.</summary>
public class AuditLogger : IAuditLogger
{
    private readonly AppDbContext _db;

    public AuditLogger(AppDbContext db) => _db = db;

    public async Task LogAsync(
        string action,
        string entityName,
        string entityId,
        string performedBy,
        string tenantId,
        string? details = null)
    {
        _db.AuditLogs.Add(new AuditLog
        {
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            PerformedBy = performedBy,
            TenantId = tenantId,
            Details = details,
            TimestampUtc = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
