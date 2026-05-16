using System.Security.Claims;
using KycCompliancePortal.Infrastructure.Security;

namespace KycCompliancePortal.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string TenantId(this ClaimsPrincipal user) =>
        user.FindFirstValue(JwtTokenGenerator.TenantClaim) ?? string.Empty;

    public static string Email(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email)
        ?? user.FindFirstValue("email")
        ?? "unknown";
}
