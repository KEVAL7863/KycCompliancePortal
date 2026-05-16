namespace KycCompliancePortal.Core.Models;

/// <summary>
/// Pure input to the risk engine. Deliberately has no EF / database dependency
/// so the scoring logic can be unit tested in isolation.
/// </summary>
public class RiskProfile
{
    public string Country { get; set; } = string.Empty;

    public bool IsPoliticallyExposed { get; set; }

    public decimal AnnualIncome { get; set; }

    public decimal ExpectedMonthlyTransactionVolume { get; set; }

    public int Age { get; set; }
}
