namespace KycCompliancePortal.Core.Interfaces;

/// <summary>
/// Writes immutable audit-trail entries. Every state-changing action in the
/// portal goes through this so compliance officers have a full history.
/// </summary>
public interface IAuditLogger
{
    Task LogAsync(
        string action,
        string entityName,
        string entityId,
        string performedBy,
        string tenantId,
        string? details = null);
}
