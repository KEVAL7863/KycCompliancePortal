using FluentAssertions;
using KycCompliancePortal.Application.Risk;
using KycCompliancePortal.Core.Enums;
using KycCompliancePortal.Core.Models;
using Xunit;

namespace KycCompliancePortal.Tests.Risk;

public class RiskScoringServiceTests
{
    private readonly RiskScoringService _sut = new();

    private static RiskProfile CleanProfile() => new()
    {
        Country = "India",
        IsPoliticallyExposed = false,
        AnnualIncome = 1_200_000m,
        ExpectedMonthlyTransactionVolume = 50_000m,
        Age = 35
    };

    [Fact]
    public void Evaluate_CleanProfile_IsLowRiskWithZeroScore()
    {
        var result = _sut.Evaluate(CleanProfile());

        result.Score.Should().Be(0);
        result.Level.Should().Be(RiskLevel.Low);
        result.Reasons.Should().ContainSingle()
            .Which.Should().Be("No elevated risk factors detected");
    }

    [Fact]
    public void Evaluate_HighRiskCountry_Adds40Points()
    {
        var profile = CleanProfile();
        profile.Country = "Iran";

        var result = _sut.Evaluate(profile);

        result.Score.Should().Be(40);
        result.Level.Should().Be(RiskLevel.Medium);
        result.Reasons.Should().ContainMatch("*high-risk jurisdiction*");
    }

    [Fact]
    public void Evaluate_PoliticallyExposedPerson_Adds30Points()
    {
        var profile = CleanProfile();
        profile.IsPoliticallyExposed = true;

        var result = _sut.Evaluate(profile);

        result.Score.Should().Be(30);
        result.Reasons.Should().ContainMatch("*Politically Exposed*");
    }

    [Fact]
    public void Evaluate_HighRiskCountryAndPep_IsHighRisk()
    {
        var profile = CleanProfile();
        profile.Country = "Syria";
        profile.IsPoliticallyExposed = true;

        var result = _sut.Evaluate(profile);

        result.Score.Should().Be(70);
        result.Level.Should().Be(RiskLevel.High);
    }

    [Fact]
    public void Evaluate_ActivityWithNoDeclaredIncome_Adds20Points()
    {
        var profile = CleanProfile();
        profile.AnnualIncome = 0m;
        profile.ExpectedMonthlyTransactionVolume = 10_000m;

        var result = _sut.Evaluate(profile);

        result.Score.Should().Be(20);
        result.Reasons.Should().ContainMatch("*no declared income*");
    }

    [Fact]
    public void Evaluate_VolumeFarExceedsIncome_Adds20Points()
    {
        var profile = CleanProfile();
        profile.AnnualIncome = 500_000m;
        profile.ExpectedMonthlyTransactionVolume = 200_000m; // 2.4M/yr > 1.5M

        var result = _sut.Evaluate(profile);

        result.Score.Should().Be(20);
        result.Reasons.Should().ContainMatch("*exceeds declared income*");
    }

    [Fact]
    public void Evaluate_VeryHighMonthlyVolume_Adds15Points()
    {
        var profile = CleanProfile();
        profile.AnnualIncome = 50_000_000m;            // large, so no income mismatch
        profile.ExpectedMonthlyTransactionVolume = 1_000_000m;

        var result = _sut.Evaluate(profile);

        result.Score.Should().Be(15);
        result.Reasons.Should().ContainMatch("*high expected monthly transaction volume*");
    }

    [Fact]
    public void Evaluate_YoungCustomer_Adds10Points()
    {
        var profile = CleanProfile();
        profile.Age = 19;

        var result = _sut.Evaluate(profile);

        result.Score.Should().Be(10);
        result.Reasons.Should().ContainMatch("*Young customer*");
    }

    [Fact]
    public void Evaluate_AllRiskFactors_ScoreIsClampedTo100()
    {
        var profile = new RiskProfile
        {
            Country = "North Korea",
            IsPoliticallyExposed = true,
            AnnualIncome = 0m,
            ExpectedMonthlyTransactionVolume = 5_000_000m,
            Age = 18
        };

        var result = _sut.Evaluate(profile);

        // 40 + 30 + 20 + 15 + 10 = 115 -> clamped
        result.Score.Should().Be(100);
        result.Level.Should().Be(RiskLevel.High);
    }

    [Theory]
    // expectedScore, expectedLevel, country, isPep, income, monthlyVolume, age
    [InlineData(0,   RiskLevel.Low,    "India",       false, 1_200_000, 50_000,    35)]
    [InlineData(10,  RiskLevel.Low,    "India",       false, 1_200_000, 50_000,    19)]
    [InlineData(30,  RiskLevel.Medium, "India",       true,  1_200_000, 50_000,    35)]
    [InlineData(40,  RiskLevel.Medium, "Iran",        false, 1_200_000, 50_000,    35)]
    [InlineData(70,  RiskLevel.High,   "Iran",        true,  1_200_000, 50_000,    35)]
    [InlineData(100, RiskLevel.High,   "North Korea", true,  0,         5_000_000, 18)]
    public void Evaluate_RuleCombinations_ProduceExpectedScoreAndLevel(
        int expectedScore, RiskLevel expectedLevel, string country,
        bool isPep, int income, int monthlyVolume, int age)
    {
        var profile = new RiskProfile
        {
            Country = country,
            IsPoliticallyExposed = isPep,
            AnnualIncome = income,
            ExpectedMonthlyTransactionVolume = monthlyVolume,
            Age = age
        };

        var result = _sut.Evaluate(profile);

        result.Score.Should().Be(expectedScore);
        result.Level.Should().Be(expectedLevel);
    }

    [Fact]
    public void Evaluate_NullProfile_Throws()
    {
        var act = () => _sut.Evaluate(null!);
        act.Should().Throw<ArgumentNullException>();
    }
}
