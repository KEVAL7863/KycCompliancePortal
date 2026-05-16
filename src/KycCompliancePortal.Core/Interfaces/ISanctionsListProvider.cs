using KycCompliancePortal.Core.Entities;

namespace KycCompliancePortal.Core.Interfaces;

/// <summary>
/// Supplies the set of names to screen against. Abstracted so the screening
/// service can be unit tested with a fake list instead of a real database.
/// </summary>
public interface ISanctionsListProvider
{
    IReadOnlyCollection<SanctionedEntity> GetAll();
}
