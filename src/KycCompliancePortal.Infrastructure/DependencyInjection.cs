using KycCompliancePortal.Application.Aml;
using KycCompliancePortal.Application.Risk;
using KycCompliancePortal.Core.Interfaces;
using KycCompliancePortal.Infrastructure.Aml;
using KycCompliancePortal.Infrastructure.Auditing;
using KycCompliancePortal.Infrastructure.Persistence;
using KycCompliancePortal.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KycCompliancePortal.Infrastructure;

/// <summary>
/// One place to wire every infrastructure + application service into DI,
/// keeping Program.cs thin.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opt =>
            opt.UseSqlite(config.GetConnectionString("Default")
                          ?? "Data Source=kyc.db"));

        services.Configure<JwtOptions>(config.GetSection(JwtOptions.SectionName));

        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IAuditLogger, AuditLogger>();

        // Application (pure domain) services
        services.AddScoped<ISanctionsListProvider, DbSanctionsListProvider>();
        services.AddScoped<IRiskScoringService, RiskScoringService>();
        services.AddScoped<IAmlScreeningService, AmlScreeningService>();

        return services;
    }
}
