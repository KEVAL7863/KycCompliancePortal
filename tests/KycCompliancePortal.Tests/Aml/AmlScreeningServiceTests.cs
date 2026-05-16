using FluentAssertions;
using KycCompliancePortal.Application.Aml;
using KycCompliancePortal.Core.Models;
using KycCompliancePortal.Tests.TestDoubles;
using Xunit;

namespace KycCompliancePortal.Tests.Aml;

public class AmlScreeningServiceTests
{
    private static AmlScreeningService BuildSut(params string[] watchlist)
        => new(new FakeSanctionsListProvider(watchlist));

    [Fact]
    public void Screen_ExactName_ReturnsExactMatchWithScore100()
    {
        var sut = BuildSut("Keval Gelani", "Ravi Mehta");

        var result = sut.Screen("Keval Gelani");

        result.IsMatch.Should().BeTrue();
        result.MatchType.Should().Be(AmlMatchType.Exact);
        result.MatchScore.Should().Be(100);
        result.MatchedName.Should().Be("Keval Gelani");
    }

    [Theory]
    [InlineData("keval gelani")]       // case-insensitive
    [InlineData("  Keval   Gelani ")]  // whitespace
    [InlineData("Keval, Gelani")]      // punctuation
    public void Screen_NormalizedVariants_StillExactMatch(string input)
    {
        var sut = BuildSut("Keval Gelani");

        var result = sut.Screen(input);

        result.IsMatch.Should().BeTrue();
        result.MatchType.Should().Be(AmlMatchType.Exact);
    }

    [Fact]
    public void Screen_MinorTypo_ReturnsFuzzyMatch()
    {
        var sut = BuildSut("Ravi Mehta");

        // one changed letter -> high similarity, below exact
        var result = sut.Screen("Ravi Mehto");

        result.IsMatch.Should().BeTrue();
        result.MatchType.Should().Be(AmlMatchType.Fuzzy);
        result.MatchScore.Should().BeInRange(85, 99);
    }

    [Fact]
    public void Screen_UnrelatedName_ReturnsNoMatch()
    {
        var sut = BuildSut("Keval Gelani", "Ravi Mehta");

        var result = sut.Screen("Suresh Iyer");

        result.IsMatch.Should().BeFalse();
        result.MatchType.Should().Be(AmlMatchType.None);
        result.MatchScore.Should().Be(0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Screen_EmptyInput_ReturnsNoMatch(string? input)
    {
        var sut = BuildSut("Ravi Mehta");

        var result = sut.Screen(input!);

        result.IsMatch.Should().BeFalse();
    }

    [Fact]
    public void Screen_SimilarityBelowThreshold_DoesNotMatch()
    {
        // Very strict threshold => only near-identical names hit.
        var sut = new AmlScreeningService(
            new FakeSanctionsListProvider("Ravi Mehta"), fuzzyThreshold: 99);

        var result = sut.Screen("Ravi Mehto");

        result.IsMatch.Should().BeFalse();
    }

    [Fact]
    public void Screen_PicksHighestScoringCandidate()
    {
        var sut = BuildSut("Ravi Mehto", "Ravi Mehta", "Kenil Shah");

        var result = sut.Screen("Ravi Mehta");

        result.MatchType.Should().Be(AmlMatchType.Exact);
        result.MatchedName.Should().Be("Ravi Mehta");
    }
}
