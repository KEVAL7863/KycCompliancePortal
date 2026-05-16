using FluentAssertions;
using KycCompliancePortal.Application.Common;
using Xunit;

namespace KycCompliancePortal.Tests.Common;

public class LevenshteinDistanceTests
{
    [Theory]
    [InlineData("", "", 0)]
    [InlineData("abc", "", 3)]
    [InlineData("", "abc", 3)]
    [InlineData("abc", "abc", 0)]
    [InlineData("kitten", "sitting", 3)]   // textbook example
    [InlineData("flaw", "lawn", 2)]
    [InlineData("gumbo", "gambol", 2)]
    public void Compute_ReturnsKnownEditDistance(string a, string b, int expected)
    {
        LevenshteinDistance.Compute(a, b).Should().Be(expected);
    }

    [Fact]
    public void Compute_IsSymmetric()
    {
        LevenshteinDistance.Compute("kitten", "sitting")
            .Should().Be(LevenshteinDistance.Compute("sitting", "kitten"));
    }

    [Fact]
    public void Similarity_IdenticalStrings_IsOne()
    {
        LevenshteinDistance.Similarity("viktor bout", "viktor bout")
            .Should().Be(1.0);
    }

    [Fact]
    public void Similarity_TwoEmptyStrings_IsOne()
    {
        LevenshteinDistance.Similarity("", "").Should().Be(1.0);
    }

    [Fact]
    public void Similarity_CompletelyDifferent_IsLow()
    {
        LevenshteinDistance.Similarity("abcd", "wxyz").Should().Be(0.0);
    }

    [Fact]
    public void Similarity_OneCharOff_IsHigh()
    {
        LevenshteinDistance.Similarity("viktor", "vikter")
            .Should().BeApproximately(0.833, 0.01);
    }
}
