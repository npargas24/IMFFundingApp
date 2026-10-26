using System.Security.Claims;
using ImfFunding.Context;
using ImfFunding.Dtos;
using ImfFunding.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Controllers;

[ApiController]
[Route("api/fundingrequests/{requestId:int}/disbursements")]
[Authorize]
public class DisbursementsController : ControllerBase
{
    private readonly FundingContext _context;

    public DisbursementsController(FundingContext context)
    {
        _context = context;
    }

    private bool IsStaff => User.IsInRole("IMFStaff");

    private int? CallerCountryId =>
        int.TryParse(User.FindFirstValue("countryId"), out var id) ? id : null;

    // Same visibility rule as the requests: reps only see their own country's
    private IQueryable<FundingRequest> VisibleRequests(int requestId)
    {
        var query = _context.FundingRequests.Where(r => r.RequestId == requestId);

        if (!IsStaff)
            query = query.Where(r => r.CountryId == CallerCountryId);

        return query;
    }

    private static DisbursementDto ToDto(Disbursement d) => new(
        d.DisbursementId, d.TrancheNo, d.AmountSDR,
        d.ScheduledDate, d.ReleasedDate, d.Status.ToString());

    // GET /api/fundingrequests/2/disbursements
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DisbursementDto>>> Get(int requestId)
    {
        if (!await VisibleRequests(requestId).AnyAsync())
            return NotFound();

        var rows = await _context.Disbursements
            .AsNoTracking()
            .Where(d => d.RequestId == requestId)
            .OrderBy(d => d.TrancheNo)
            .ToListAsync();

        return Ok(rows.Select(ToDto));
    }

    // POST /api/fundingrequests/2/disbursements  (staff only)
    [HttpPost]
    [Authorize(Roles = "IMFStaff")]
    public async Task<ActionResult<DisbursementDto>> Schedule(int requestId, CreateDisbursementDto dto)
    {
        var request = await VisibleRequests(requestId).FirstOrDefaultAsync();
        if (request is null)
            return NotFound();

        if (request.Status != RequestStatus.Approved)
            return Conflict(new ProblemDetails { Title = "Disbursements can only be scheduled for Approved requests." });

        var existing = await _context.Disbursements
            .Where(d => d.RequestId == requestId)
            .ToListAsync();

        if (existing.Sum(d => d.AmountSDR) + dto.AmountSDR > (request.ApprovedAmountSDR ?? 0))
            return UnprocessableEntity(new ProblemDetails
            {
                Title = "Total disbursements cannot exceed the approved amount."
            });

        var disbursement = new Disbursement
        {
            RequestId = requestId,
            TrancheNo = existing.Count == 0 ? 1 : existing.Max(d => d.TrancheNo) + 1,   // server numbers the tranches
            AmountSDR = dto.AmountSDR,
            ScheduledDate = dto.ScheduledDate!.Value
        };

        _context.Disbursements.Add(disbursement);
        await _context.SaveChangesAsync();

        return Created($"/api/fundingrequests/{requestId}/disbursements", ToDto(disbursement));
    }

    // POST /api/fundingrequests/2/disbursements/1/release  (staff only)
    [HttpPost("{disbursementId:int}/release")]
    [Authorize(Roles = "IMFStaff")]
    public async Task<ActionResult<DisbursementDto>> Release(int requestId, int disbursementId)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var request = await VisibleRequests(requestId).FirstOrDefaultAsync();
        if (request is null)
            return NotFound();

        var tranches = await _context.Disbursements
            .Where(d => d.RequestId == requestId)
            .ToListAsync();

        var tranche = tranches.FirstOrDefault(d => d.DisbursementId == disbursementId);
        if (tranche is null)
            return NotFound();

        if (tranche.Status == DisbursementStatus.Released)
            return Conflict(new ProblemDetails { Title = "This tranche has already been released." });

        tranche.Status = DisbursementStatus.Released;
        tranche.ReleasedDate = DateOnly.FromDateTime(DateTime.UtcNow);

        // When every tranche is released and together they cover the approved amount, the request is Completed
        var fullyCovered = tranches.Sum(d => d.AmountSDR) == request.ApprovedAmountSDR;
        if (fullyCovered && tranches.All(d => d.Status == DisbursementStatus.Released))
        {
            request.Status = RequestStatus.Completed;

            _context.RequestStatusHistories.Add(new RequestStatusHistory
            {
                RequestId = request.RequestId,
                FromStatus = RequestStatus.Approved,
                ToStatus = RequestStatus.Completed,
                ChangedBy = userId,
                Comment = "All tranches released"
            });
        }

        // One save writes the release and, if it applies, the Completed status and its history row
        await _context.SaveChangesAsync();

        return Ok(ToDto(tranche));
    }
}