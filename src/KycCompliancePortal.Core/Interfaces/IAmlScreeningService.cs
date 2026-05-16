using KycCompliancePortal.Core.Models;

namespace KycCompliancePortal.Core.Interfaces;

/// <summary>
/// Screens a customer name against the sanctions watchlist using exact and
/// fuzzy (edit-distance) matching.
/// </summary>
public interface IAmlScreeningService
{
    AmlScreeningResult Screen(string fullName);
}
