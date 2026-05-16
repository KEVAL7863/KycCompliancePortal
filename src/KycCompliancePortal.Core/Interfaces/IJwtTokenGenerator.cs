using KycCompliancePortal.Core.Entities;

namespace KycCompliancePortal.Core.Interfaces;

/// <summary>Issues a signed JWT carrying the user's id, role and tenant.</summary>
public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
