using KycCompliancePortal.Core.Enums;

namespace KycCompliancePortal.Core.Entities;

/// <summary>
/// A single identity / address proof uploaded against a customer.
/// </summary>
public class KycDocument
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public Customer? Customer { get; set; }

    public DocumentType DocumentType { get; set; }

    public string FileName { get; set; } = string.Empty;

    public KycStatus Status { get; set; } = KycStatus.Pending;

    public DateTime UploadedAtUtc { get; set; } = DateTime.UtcNow;
}
