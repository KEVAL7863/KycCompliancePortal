using System.ComponentModel.DataAnnotations;

namespace KycCompliancePortal.Api.Contracts;

public class CreateCustomerRequest
{
    [Required]
    public string FullName { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Country { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [Range(0, double.MaxValue)]
    public decimal AnnualIncome { get; set; }

    [Range(0, double.MaxValue)]
    public decimal ExpectedMonthlyTransactionVolume { get; set; }

    public bool IsPoliticallyExposed { get; set; }
}

public class CustomerResponse
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RiskScore { get; set; }
    public string RiskLevel { get; set; } = string.Empty;
    public IReadOnlyList<string> RiskReasons { get; set; } = Array.Empty<string>();
    public bool AmlHit { get; set; }
    public string? AmlMatchedName { get; set; }
    public string AmlMatchType { get; set; } = "None";
}
