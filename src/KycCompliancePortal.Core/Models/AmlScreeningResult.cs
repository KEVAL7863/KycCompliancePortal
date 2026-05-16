namespace KycCompliancePortal.Core.Models;

/// <summary>How a customer name matched (or did not) against the watchlist.</summary>
public enum AmlMatchType
{
    None = 0,
    Fuzzy = 1,
    Exact = 2
}

/// <summary>
/// Result of screening a single name against the sanctions list.
/// </summary>
public class AmlScreeningResult
{
    public bool IsMatch { get; }

    /// <summary>The watchlist name that matched, if any.</summary>
    public string? MatchedName { get; }

    /// <summary>Similarity score 0-100 (100 = exact).</summary>
    public int MatchScore { get; }

    public AmlMatchType MatchType { get; }

    public AmlScreeningResult(bool isMatch, string? matchedName, int matchScore, AmlMatchType matchType)
    {
        IsMatch = isMatch;
        MatchedName = matchedName;
        MatchScore = matchScore;
        MatchType = matchType;
    }

    public static AmlScreeningResult NoMatch() => new(false, null, 0, AmlMatchType.None);
}
