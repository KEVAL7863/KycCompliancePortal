using KycCompliancePortal.Api.Contracts;
using KycCompliancePortal.Core.Entities;
using KycCompliancePortal.Core.Interfaces;
using KycCompliancePortal.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KycCompliancePortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _tokens;

    public AuthController(AppDbContext db, IPasswordHasher hasher, IJwtTokenGenerator tokens)
    {
        _db = db;
        _hasher = hasher;
        _tokens = tokens;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        bool exists = await _db.Users.AnyAsync(
            u => u.TenantId == request.TenantId && u.Email == request.Email);
        if (exists)
            return Conflict(new { message = "A user with this email already exists for the tenant." });

        var user = new User
        {
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password),
            Role = request.Role,
            TenantId = request.TenantId
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(BuildResponse(user));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null || !_hasher.Verify(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid credentials." });

        return Ok(BuildResponse(user));
    }

    private AuthResponse BuildResponse(User user) => new()
    {
        Token = _tokens.GenerateToken(user),
        Email = user.Email,
        Role = user.Role.ToString(),
        TenantId = user.TenantId
    };
}
