namespace KycCompliancePortal.Core.Entities;

/// <summary>
/// Immutable audit trail row. Compliance systems must record who did what and when;
/// rows are only ever inserted, never updated or deleted.
/// </summary>
public class AuditLog
{
    public int Id { get; set; }

    /// <summary>E.g. "CustomerCreated", "KycApproved", "AmlHit".</summary>
    public string Action { get; set; } = string.Empty;

    public string EntityName { get; set; } = string.Empty;

    public string EntityId { get; set; } = string.Empty;

    /// <summary>Email of the user who performed the action (or "system").</summary>
    public string PerformedBy { get; set; } = string.Empty;

    public string TenantId { get; set; } = string.Empty;

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public string? Details { get; set; }
}
