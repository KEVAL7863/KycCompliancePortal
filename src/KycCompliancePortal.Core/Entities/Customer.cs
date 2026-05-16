using KycCompliancePortal.Core.Enums;

namespace KycCompliancePortal.Core.Entities;

/// <summary>
/// A customer being onboarded. Carries the data the risk engine and AML
/// screening operate on, plus the computed risk outcome.
/// </summary>
public class Customer
{
    public int Id { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    /// <summary>Country of residence (ISO name, e.g. "India", "Iran").</summary>
    public string Country { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    public decimal AnnualIncome { get; set; }

    public decimal ExpectedMonthlyTransactionVolume { get; set; }

    /// <summary>Politically Exposed Person — a key AML risk factor.</summary>
    public bool IsPoliticallyExposed { get; set; }

    public KycStatus Status { get; set; } = KycStatus.Pending;

    public int RiskScore { get; set; }

    public RiskLevel RiskLevel { get; set; } = RiskLevel.Low;

    /// <summary>Owning bank (multi-tenancy key).</summary>
    public string TenantId { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<KycDocument> Documents { get; set; } = new List<KycDocument>();
}
