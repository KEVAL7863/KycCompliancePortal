using KycCompliancePortal.Core.Models;

namespace KycCompliancePortal.Core.Interfaces;

/// <summary>
/// Computes a customer's money-laundering risk from their profile.
/// Implemented with pure, side-effect-free logic so it is fully unit testable.
/// </summary>
public interface IRiskScoringService
{
    RiskAssessmentResult Evaluate(RiskProfile profile);
}
