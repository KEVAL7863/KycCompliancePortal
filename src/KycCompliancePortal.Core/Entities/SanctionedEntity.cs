namespace KycCompliancePortal.Core.Entities;

/// <summary>
/// One name on a sanctions / watchlist (e.g. OFAC SDN, UN Consolidated List).
/// The AML screening service matches incoming customer names against these.
/// </summary>
public class SanctionedEntity
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    /// <summary>Which list this name came from, e.g. "OFAC", "UN", "EU".</summary>
    public string ListSource { get; set; } = string.Empty;
}
