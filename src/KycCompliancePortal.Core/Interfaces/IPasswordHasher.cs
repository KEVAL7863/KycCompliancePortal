namespace KycCompliancePortal.Core.Interfaces;

/// <summary>Hashes and verifies passwords. Plain passwords never leave this boundary.</summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
