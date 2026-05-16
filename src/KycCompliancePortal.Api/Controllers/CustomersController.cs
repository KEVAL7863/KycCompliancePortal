using KycCompliancePortal.Api.Contracts;
using KycCompliancePortal.Api.Extensions;
using KycCompliancePortal.Core.Entities;
using KycCompliancePortal.Core.Enums;
using KycCompliancePortal.Core.Interfaces;
using KycCompliancePortal.Core.Models;
using KycCompliancePortal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KycCompliancePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : ControllerBase
{
    private const string ReviewerRoles = nameof(UserRole.ComplianceOfficer) + "," + nameof(UserRole.Admin);

    private readonly AppDbContext _db;
    private readonly IRiskScoringService _risk;
    private readonly IAmlScreeningService _aml;
    private readonly IAuditLogger _audit;

    public CustomersController(
        AppDbContext db,
        IRiskScoringService risk,
        IAmlScreeningService aml,
        IAuditLogger audit)
    {
        _db = db;
        _risk = risk;
        _aml = aml;
        _audit = audit;
    }

    /// <summary>Onboard a customer. Runs risk scoring + AML screening synchronously.</summary>
    [HttpPost]
    public async Task<ActionResult<CustomerResponse>> Create(CreateCustomerRequest request)
    {
        string tenant = User.TenantId();

        var profile = new RiskProfile
        {
            Country = request.Country,
            IsPoliticallyExposed = request.IsPoliticallyExposed,
            AnnualIncome = request.AnnualIncome,
            ExpectedMonthlyTransactionVolume = request.ExpectedMonthlyTransactionVolume,
            Age = CalculateAge(request.DateOfBirth)
        };

        RiskAssessmentResult risk = _risk.Evaluate(profile);
        AmlScreeningResult aml = _aml.Screen(request.FullName);

        // High risk or any watchlist hit => needs a human reviewer.
        KycStatus status = (aml.IsMatch || risk.Level == RiskLevel.High)
            ? KycStatus.UnderReview
            : KycStatus.Pending;

        var customer = new Customer
        {
            FullName = request.FullName,
            Email = request.Email,
            Country = request.Country,
            DateOfBirth = request.DateOfBirth,
            AnnualIncome = request.AnnualIncome,
            ExpectedMonthlyTransactionVolume = request.ExpectedMonthlyTransactionVolume,
            IsPoliticallyExposed = request.IsPoliticallyExposed,
            RiskScore = risk.Score,
            RiskLevel = risk.Level,
            Status = status,
            TenantId = tenant
        };

        _db.Customers.Add(customer);
        await _db.SaveChangesAsync();

        await _audit.LogAsync("CustomerCreated", nameof(Customer), customer.Id.ToString(),
            User.Email(), tenant,
            $"RiskScore={risk.Score}, Level={risk.Level}, Status={status}");

        if (aml.IsMatch)
        {
            await _audit.LogAsync("AmlHit", nameof(Customer), customer.Id.ToString(),
                User.Email(), tenant,
                $"Matched '{aml.MatchedName}' ({aml.MatchType}, score {aml.MatchScore})");
        }

        return CreatedAtAction(nameof(GetById), new { id = customer.Id },
            ToResponse(customer, risk.Reasons, aml));
    }

    [HttpGet]
    [Authorize(Roles = ReviewerRoles)]
    public async Task<ActionResult<IEnumerable<CustomerResponse>>> GetAll()
    {
        string tenant = User.TenantId();
        var customers = await _db.Customers
            .AsNoTracking()
            .Where(c => c.TenantId == tenant)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync();

        return Ok(customers.Select(c => ToResponse(c, Array.Empty<string>(), null)));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerResponse>> GetById(int id)
    {
        string tenant = User.TenantId();
        var customer = await _db.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenant);

        return customer is null
            ? NotFound()
            : Ok(ToResponse(customer, Array.Empty<string>(), null));
    }

    [HttpPost("{id:int}/approve")]
    [Authorize(Roles = ReviewerRoles)]
    public Task<ActionResult<CustomerResponse>> Approve(int id) =>
        ChangeStatus(id, KycStatus.Approved, "KycApproved");

    [HttpPost("{id:int}/reject")]
    [Authorize(Roles = ReviewerRoles)]
    public Task<ActionResult<CustomerResponse>> Reject(int id) =>
        ChangeStatus(id, KycStatus.Rejected, "KycRejected");

    private async Task<ActionResult<CustomerResponse>> ChangeStatus(
        int id, KycStatus newStatus, string action)
    {
        string tenant = User.TenantId();
        var customer = await _db.Customers
            .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenant);
        if (customer is null)
            return NotFound();

        customer.Status = newStatus;
        await _db.SaveChangesAsync();

        await _audit.LogAsync(action, nameof(Customer), customer.Id.ToString(),
            User.Email(), tenant, $"Status set to {newStatus}");

        return Ok(ToResponse(customer, Array.Empty<string>(), null));
    }

    private static int CalculateAge(DateTime dob)
    {
        if (dob == default) return 0;
        var today = DateTime.UtcNow;
        int age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }

    private static CustomerResponse ToResponse(
        Customer c, IReadOnlyList<string> reasons, AmlScreeningResult? aml) => new()
    {
        Id = c.Id,
        FullName = c.FullName,
        Email = c.Email,
        Country = c.Country,
        Status = c.Status.ToString(),
        RiskScore = c.RiskScore,
        RiskLevel = c.RiskLevel.ToString(),
        RiskReasons = reasons,
        AmlHit = aml?.IsMatch ?? false,
        AmlMatchedName = aml?.MatchedName,
        AmlMatchType = (aml?.MatchType ?? AmlMatchType.None).ToString()
    };
}
