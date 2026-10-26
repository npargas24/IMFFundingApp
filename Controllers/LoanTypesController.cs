using ImfFunding.Context;
using ImfFunding.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoanTypesController : ControllerBase
{
    private readonly FundingContext _context;

    public LoanTypesController(FundingContext context)
    {
        _context = context;
    }

    // GET /api/loantypes
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanTypeDto>>> GetAll()
    {
        var loanTypes = await _context.LoanTypes
            .Where(l => l.IsActive)
            .OrderBy(l => l.Name)
            .Select(l => new LoanTypeDto(l.LoanTypeId, l.Code, l.Name, l.Description))
            .ToListAsync();

        return Ok(loanTypes);
    }

    // GET /api/loantypes/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<LoanTypeDto>> GetById(int id)
    {
        var loanType = await _context.LoanTypes
            .Where(l => l.LoanTypeId == id && l.IsActive)
            .Select(l => new LoanTypeDto(l.LoanTypeId, l.Code, l.Name, l.Description))
            .FirstOrDefaultAsync();

        return loanType is null ? NotFound() : Ok(loanType);
    }
}