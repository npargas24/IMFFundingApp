using System.Security.Claims;
using ImfFunding.Context;
using ImfFunding.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly FundingContext _context;

    public UsersController(FundingContext context)
    {
        _context = context;
    }

    // GET /api/users/me
    [HttpGet("currentUser")]
    public async Task<ActionResult<ApplicationUserDto>> GetCurrentUser()
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var userId))
            return Unauthorized();

        var user = await _context.ApplicationUsers
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

        if (user is null)
            return Unauthorized();

        return Ok(new ApplicationUserDto(user.UserId, user.Email, user.FullName, user.Role.ToString(), user.CountryId));
    }
}