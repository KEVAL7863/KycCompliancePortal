using KycCompliancePortal.Core.Enums;

namespace KycCompliancePortal.Core.Models;

/// <summary>
/// Output of the risk engine: a 0-100 score, the bucket it falls in,
/// and a human-readable explanation of every factor that contributed.
/// </summary>
public class RiskAssessmentResult
{
    public int Score { get; }

    public RiskLevel Level { get; }

    public IReadOnlyList<string> Reasons { get; }

    public RiskAssessmentResult(int score, RiskLevel level, IReadOnlyList<string> reasons)
    {
        Score = score;
        Level = level;
        Reasons = reasons;
    }
}
