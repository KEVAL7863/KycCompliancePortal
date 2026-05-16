using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using KycCompliancePortal.Core.Entities;
using KycCompliancePortal.Core.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KycCompliancePortal.Infrastructure.Security;

/// <summary>
/// Builds a signed JWT containing the user id, email, role and tenant.
/// The tenant claim is what downstream queries use to isolate data.
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    public const string TenantClaim = "tenant";

    private readonly JwtOptions _options;

    public JwtTokenGenerator(IOptions<JwtOptions> options) => _options = options.Value;

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(TenantClaim, user.TenantId)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
