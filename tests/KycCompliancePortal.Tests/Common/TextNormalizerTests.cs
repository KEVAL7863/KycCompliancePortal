using FluentAssertions;
using KycCompliancePortal.Application.Common;
using Xunit;

namespace KycCompliancePortal.Tests.Common;

public class TextNormalizerTests
{
    [Theory]
    [InlineData("John Doe", "john doe")]
    [InlineData("  John   Doe  ", "john doe")]
    [InlineData("O'Brien, John", "o brien john")]
    [InlineData("JOHN-DOE", "john doe")]
    [InlineData("Jöhn", "jöhn")]
    public void Normalize_CleansAndLowercases(string input, string expected)
    {
        TextNormalizer.Normalize(input).Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("!!!")]
    public void Normalize_NullOrPunctuationOnly_ReturnsEmpty(string? input)
    {
        TextNormalizer.Normalize(input).Should().BeEmpty();
    }
}
