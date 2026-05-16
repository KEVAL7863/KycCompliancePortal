using KycCompliancePortal.Core.Enums;

namespace KycCompliancePortal.Core.Entities;

/// <summary>
/// An authenticated user of the portal. A user always belongs to a tenant (a bank),
/// which is the basis for multi-tenant data isolation.
/// </summary>
public class User
{
    public int Id { get; set; }

    public string Email { get; set; } = string.Empty;

    /// <summary>BCrypt hash — the plain password is never stored.</summary>
    public string PasswordHash { get; set; } = string.Empty;

    public UserRole Role { get; set; } = UserRole.Customer;

    /// <summary>Identifier of the bank this user belongs to (multi-tenancy key).</summary>
    public string TenantId { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
