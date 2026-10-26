using ImfFunding.Context;
using ImfFunding.Dtos;
using ImfFunding.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ImfFunding.Services;

namespace ImfFunding.Controllers;

[ApiController]
[Route("api/auth")]
public class UserAuthController : ControllerBase
{
    private readonly FundingContext _context;
    private readonly ITokenService _tokenService;
    private readonly PasswordHasher<ApplicationUser> _hasher = new();

    public UserAuthController(FundingContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApplicationUserDto>> Register(RegisterDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _context.ApplicationUsers.AnyAsync(u => u.Email == email))
            return Conflict(new ProblemDetails { Title = "Email already registered." });

        var countryOk = await _context.Countries.AnyAsync(c => c.CountryId == dto.CountryId && c.IsActive);
        if (!countryOk)
            return BadRequest(new ProblemDetails { Title = "Country not found." });

        var user = new ApplicationUser
        {
            Email = email,
            FullName = dto.FullName,
            Role = UserRole.CountryRep,
            CountryId = dto.CountryId
        };
        user.PasswordHash = _hasher.HashPassword(user, dto.Password);

        _context.ApplicationUsers.Add(user);
        await _context.SaveChangesAsync();

        var result = new ApplicationUserDto(user.UserId, user.Email, user.FullName, user.Role.ToString(), user.CountryId);
        return Created($"/api/users/{user.UserId}", result);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDto dto)
    {
        // Lower Invariant prevents duplicate accounts with the same email but different casing, e.g. "john@example.com" and "JOHN@EXAMPLE.COM"
        var email = dto.Email.Trim().ToLowerInvariant();

        var user = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);

        // Same message whether the email or the password was wrong
        if (user is null)
            return Unauthorized(new ProblemDetails { Title = "Invalid email or password." });

        var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
        if (verify == PasswordVerificationResult.Failed)
            return Unauthorized(new ProblemDetails { Title = "Invalid email or password." });

        var token = _tokenService.CreateToken(user);
        var userDto = new ApplicationUserDto(user.UserId, user.Email, user.FullName, user.Role.ToString(), user.CountryId);

        return Ok(new LoginResponseDto(token, userDto));
    }
}