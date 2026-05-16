using KycCompliancePortal.Core.Enums;
using KycCompliancePortal.Core.Interfaces;
using KycCompliancePortal.Core.Models;

namespace KycCompliancePortal.Application.Risk;

/// <summary>
/// Rule-based money-laundering risk engine.
///
/// Each rule adds points and a human-readable reason. The total is clamped to
/// 0-100 and bucketed into Low / Medium / High. Pure and deterministic — no
/// database, no clock, no randomness — which is exactly why it is easy to
/// drive with unit tests / TDD.
/// </summary>
public class RiskScoringService : IRiskScoringService
{
    private readonly RiskScoringOptions _options;

    public RiskScoringService() : this(new RiskScoringOptions()) { }

    public RiskScoringService(RiskScoringOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public RiskAssessmentResult Evaluate(RiskProfile profile)
    {
        ArgumentNullException.ThrowIfNull(profile);

        int score = 0;
        var reasons = new List<string>();

        if (!string.IsNullOrWhiteSpace(profile.Country) &&
            _options.HighRiskCountries.Contains(profile.Country.Trim()))
        {
            score += 40;
            reasons.Add($"Resident of high-risk jurisdiction: {profile.Country.Trim()}");
        }

        if (profile.IsPoliticallyExposed)
        {
            score += 30;
            reasons.Add("Politically Exposed Person (PEP)");
        }

        decimal expectedAnnual = profile.ExpectedMonthlyTransactionVolume * 12;
        if (profile.AnnualIncome <= 0 && profile.ExpectedMonthlyTransactionVolume > 0)
        {
            score += 20;
            reasons.Add("Expected transaction activity with no declared income");
        }
        else if (profile.AnnualIncome > 0 && expectedAnnual > profile.AnnualIncome * 3)
        {
            score += 20;
            reasons.Add("Expected transaction volume far exceeds declared income");
        }

        if (profile.ExpectedMonthlyTransactionVolume >= _options.HighMonthlyVolumeThreshold)
        {
            score += 15;
            reasons.Add("Very high expected monthly transaction volume");
        }

        if (profile.Age > 0 && profile.Age < _options.YoungAgeThreshold)
        {
            score += 10;
            reasons.Add("Young customer with elevated transaction activity");
        }

        score = Math.Clamp(score, 0, 100);

        RiskLevel level = score >= 70 ? RiskLevel.High
                        : score >= 30 ? RiskLevel.Medium
                        : RiskLevel.Low;

        if (reasons.Count == 0)
            reasons.Add("No elevated risk factors detected");

        return new RiskAssessmentResult(score, level, reasons);
    }
}
