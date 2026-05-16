using KycCompliancePortal.Core.Interfaces;

namespace KycCompliancePortal.Infrastructure.Security;

/// <summary>BCrypt-backed password hasher (work factor 11).</summary>
public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password) =>
        BCrypt.Net.BCrypt.HashPassword(password, workFactor: 11);

    public bool Verify(string password, string hash) =>
        BCrypt.Net.BCrypt.Verify(password, hash);
}
