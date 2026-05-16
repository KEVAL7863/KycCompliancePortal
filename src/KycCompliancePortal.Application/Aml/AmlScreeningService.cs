using KycCompliancePortal.Application.Common;
using KycCompliancePortal.Core.Interfaces;
using KycCompliancePortal.Core.Models;

namespace KycCompliancePortal.Application.Aml;

/// <summary>
/// Screens a customer name against a sanctions watchlist.
///
/// Strategy:
///   1. Normalize both sides (case, punctuation, whitespace).
///   2. Exact match  -> score 100, MatchType = Exact.
///   3. Otherwise take the best Levenshtein similarity across the list;
///      if it meets the configured threshold it is a Fuzzy hit.
///
/// The watchlist is supplied via <see cref="ISanctionsListProvider"/>, so this
/// service is unit tested with an in-memory fake — no database required.
/// </summary>
public class AmlScreeningService : IAmlScreeningService
{
    private readonly ISanctionsListProvider _sanctionsList;
    private readonly int _fuzzyThreshold;

    /// <param name="fuzzyThreshold">
    /// Minimum similarity (0-100) for a fuzzy hit. Default 85.
    /// </param>
    public AmlScreeningService(ISanctionsListProvider sanctionsList, int fuzzyThreshold = 85)
    {
        _sanctionsList = sanctionsList ?? throw new ArgumentNullException(nameof(sanctionsList));
        _fuzzyThreshold = fuzzyThreshold;
    }

    public AmlScreeningResult Screen(string fullName)
    {
        string candidate = TextNormalizer.Normalize(fullName);
        if (candidate.Length == 0)
            return AmlScreeningResult.NoMatch();

        AmlScreeningResult? bestFuzzy = null;

        foreach (var entity in _sanctionsList.GetAll())
        {
            string listName = TextNormalizer.Normalize(entity.FullName);
            if (listName.Length == 0)
                continue;

            if (listName == candidate)
                return new AmlScreeningResult(true, entity.FullName, 100, AmlMatchType.Exact);

            int similarity = (int)Math.Round(
                LevenshteinDistance.Similarity(candidate, listName) * 100);

            if (similarity >= _fuzzyThreshold &&
                (bestFuzzy is null || similarity > bestFuzzy.MatchScore))
            {
                bestFuzzy = new AmlScreeningResult(true, entity.FullName, similarity, AmlMatchType.Fuzzy);
            }
        }

        return bestFuzzy ?? AmlScreeningResult.NoMatch();
    }
}
