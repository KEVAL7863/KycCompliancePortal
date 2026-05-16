namespace KycCompliancePortal.Application.Risk;

/// <summary>
/// Tunable thresholds for the risk engine. Defaults are sensible for a demo;
/// in production these would come from configuration / per-tenant policy.
/// </summary>
public class RiskScoringOptions
{
    public ISet<string> HighRiskCountries { get; set; } =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Iran", "North Korea", "Syria", "Myanmar", "Afghanistan", "Sudan"
        };

    /// <summary>Monthly volume at or above which extra risk is added.</summary>
    public decimal HighMonthlyVolumeThreshold { get; set; } = 1_000_000m;

    /// <summary>Customers younger than this with activity are flagged.</summary>
    public int YoungAgeThreshold { get; set; } = 21;
}
