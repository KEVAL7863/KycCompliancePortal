namespace KycCompliancePortal.Core.Enums;

/// <summary>
/// Application roles. Drives JWT claims and [Authorize(Roles = ...)] on controllers.
/// </summary>
public enum UserRole
{
    Customer = 0,
    ComplianceOfficer = 1,
    Admin = 2
}

/// <summary>
/// Lifecycle of a customer's KYC verification (and of an individual document).
/// </summary>
public enum KycStatus
{
    Pending = 0,
    UnderReview = 1,
    Approved = 2,
    Rejected = 3
}

/// <summary>
/// Risk bucket derived from the numeric risk score produced by the risk engine.
/// </summary>
public enum RiskLevel
{
    Low = 0,
    Medium = 1,
    High = 2
}

/// <summary>
/// Types of identity / address proof a customer can upload.
/// </summary>
public enum DocumentType
{
    Passport = 0,
    NationalId = 1,
    DrivingLicense = 2,
    UtilityBill = 3,
    BankStatement = 4
}
