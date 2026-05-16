using System.Text;

namespace KycCompliancePortal.Application.Common;

/// <summary>
/// Normalizes names before comparison: trims, lower-cases, and treats any
/// punctuation as a word separator before collapsing runs of whitespace to a
/// single space. This makes "O'Brien,  John", "Bin-Laden" and friends compare
/// the way a human reviewer would expect ("o brien john", "bin laden").
/// </summary>
public static class TextNormalizer
{
    public static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var sb = new StringBuilder(value.Length);
        bool lastWasSpace = false;

        foreach (char c in value)
        {
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(char.ToLowerInvariant(c));
                lastWasSpace = false;
            }
            else if (!lastWasSpace)
            {
                // whitespace OR punctuation -> a single separating space
                sb.Append(' ');
                lastWasSpace = true;
            }
        }

        return sb.ToString().Trim();
    }
}
