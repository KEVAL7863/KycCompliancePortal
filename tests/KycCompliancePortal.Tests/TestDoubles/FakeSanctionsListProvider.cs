using KycCompliancePortal.Core.Entities;
using KycCompliancePortal.Core.Interfaces;

namespace KycCompliancePortal.Tests.TestDoubles;

/// <summary>
/// In-memory sanctions list so AML screening can be tested without a database.
/// </summary>
public class FakeSanctionsListProvider : ISanctionsListProvider
{
    private readonly List<SanctionedEntity> _entities;

    public FakeSanctionsListProvider(params string[] names)
    {
        _entities = names
            .Select((n, i) => new SanctionedEntity
            {
                Id = i + 1,
                FullName = n,
                Country = "Unknown",
                ListSource = "TEST"
            })
            .ToList();
    }

    public IReadOnlyCollection<SanctionedEntity> GetAll() => _entities;
}
