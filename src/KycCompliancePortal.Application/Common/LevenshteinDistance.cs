namespace KycCompliancePortal.Application.Common;

/// <summary>
/// Classic edit-distance (Levenshtein) used for fuzzy name matching.
///
/// Complexity:
///   Time : O(m * n)  — every cell of the DP table is computed once.
///   Space: O(min(m, n)) — only the previous and current rows are kept,
///           instead of the full m x n matrix (rolling-array optimization).
///
/// This is a good Round-3 talking point: start from the O(m*n) matrix,
/// then show how the rolling array reduces memory without changing the answer.
/// </summary>
public static class LevenshteinDistance
{
    public static int Compute(string source, string target)
    {
        source ??= string.Empty;
        target ??= string.Empty;

        if (source.Length == 0) return target.Length;
        if (target.Length == 0) return source.Length;

        // Keep the shorter string on the inner axis to minimize memory.
        if (source.Length < target.Length)
            (source, target) = (target, source);

        int n = target.Length;
        var previous = new int[n + 1];
        var current = new int[n + 1];

        for (int j = 0; j <= n; j++)
            previous[j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            current[0] = i;

            for (int j = 1; j <= n; j++)
            {
                int cost = source[i - 1] == target[j - 1] ? 0 : 1;
                current[j] = Math.Min(
                    Math.Min(current[j - 1] + 1,   // insertion
                             previous[j] + 1),      // deletion
                    previous[j - 1] + cost);        // substitution
            }

            (previous, current) = (current, previous);
        }

        return previous[n];
    }

    /// <summary>
    /// Similarity in [0.0, 1.0]: 1.0 means identical, 0.0 means completely different.
    /// </summary>
    public static double Similarity(string a, string b)
    {
        a ??= string.Empty;
        b ??= string.Empty;

        if (a.Length == 0 && b.Length == 0) return 1.0;

        int distance = Compute(a, b);
        int maxLen = Math.Max(a.Length, b.Length);
        return 1.0 - (double)distance / maxLen;
    }
}
