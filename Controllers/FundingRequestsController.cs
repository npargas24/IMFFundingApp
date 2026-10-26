using System.Security.Claims;
using ImfFunding.Context;
using ImfFunding.Dtos;
using ImfFunding.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FundingRequestsController : ControllerBase
{
    private readonly FundingContext _context;

    public FundingRequestsController(FundingContext context)
    {
        _context = context;
    }

    private bool IsStaff => User.IsInRole("IMFStaff");

    private int? CallerCountryId =>
        int.TryParse(User.FindFirstValue("countryId"), out var id) ? id : null;

    private static FundingRequestDto ToDto(FundingRequest r) => new(
        r.RequestId,
        r.CountryId, r.Country.Name,
        r.LoanTypeId, r.LoanType.Code,
        r.Status.ToString(), r.RequestedAmountSDR, r.ApprovedAmountSDR,
        r.Purpose, r.CreatedAt);

    // The whole status flow: who can move a request from one status to another
    private static bool IsAllowed(bool isStaff, RequestStatus from, RequestStatus to)
    {
        if (isStaff)
        {
            return (from, to) switch
            {
                (RequestStatus.Submitted, RequestStatus.UnderNegotiation) => true,
                (RequestStatus.UnderNegotiation, RequestStatus.BoardReview) => true,
                (RequestStatus.BoardReview, RequestStatus.Approved) => true,
                (RequestStatus.Submitted or RequestStatus.UnderNegotiation or RequestStatus.BoardReview,
                    RequestStatus.Rejected) => true,
                _ => false
            };
        }

        // A CountryRep can only withdraw, and only before approval
        return to == RequestStatus.Withdrawn &&
               from is RequestStatus.Submitted or RequestStatus.UnderNegotiation or RequestStatus.BoardReview;
    }

    // POST /api/fundingrequests  (CountryRep only)
    [HttpPost]
    [Authorize(Roles = "CountryRep")]
    public async Task<ActionResult<FundingRequestDto>> Create(CreateFundingRequestDto dto)
    {
        // Who is calling and which country they belong to come from the token, never the body
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId) ||
            !int.TryParse(User.FindFirstValue("countryId"), out var countryId))
            return Unauthorized();

        var country = await _context.Countries
            .FirstOrDefaultAsync(c => c.CountryId == countryId && c.IsActive);
        if (country is null)
            return BadRequest(new ProblemDetails { Title = "Your country is not active." });

        var loanType = await _context.LoanTypes
            .FirstOrDefaultAsync(l => l.LoanTypeId == dto.LoanTypeId && l.IsActive);
        if (loanType is null)
            return BadRequest(new ProblemDetails { Title = "Loan type not found." });

        var request = new FundingRequest
        {
            CountryId = countryId,
            LoanTypeId = loanType.LoanTypeId,
            Status = RequestStatus.Submitted,
            RequestedAmountSDR = dto.RequestedAmountSDR,
            Purpose = dto.Purpose.Trim(),
            CreatedBy = userId
        };

        // Linking through the navigation property lets EF fill in RequestId itself
        _context.RequestStatusHistories.Add(new RequestStatusHistory
        {
            Request = request,
            FromStatus = null,                       // first row has no previous status
            ToStatus = RequestStatus.Submitted,
            ChangedBy = userId,
            Comment = "Request submitted"
        });

        // One save writes both rows in a single transaction
        await _context.SaveChangesAsync();

        var result = new FundingRequestDto(
            request.RequestId,
            country.CountryId, country.Name,
            loanType.LoanTypeId, loanType.Code,
            request.Status.ToString(), request.RequestedAmountSDR, request.ApprovedAmountSDR,
            request.Purpose, request.CreatedAt);

        return Created($"/api/fundingrequests/{request.RequestId}", result);
    }

    // GET /api/fundingrequests/1
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FundingRequestDto>> GetById(int id)
    {
        var query = _context.FundingRequests
            .AsNoTracking()
            .Include(r => r.Country)
            .Include(r => r.LoanType)
            .Where(r => r.RequestId == id);

        // A rep can only see their own country's requests
        if (!IsStaff)
            query = query.Where(r => r.CountryId == CallerCountryId);

        var request = await query.FirstOrDefaultAsync();

        // 404 whether it doesn't exist or belongs to another country
        return request is null ? NotFound() : Ok(ToDto(request));
    }

    // GET /api/fundingrequests?status=Submitted&loanTypeId=1
    [HttpGet]
    public async Task<ActionResult<IEnumerable<FundingRequestDto>>> GetAll(
        [FromQuery] int? countryId,
        [FromQuery] int? loanTypeId,
        [FromQuery] RequestStatus? status)
    {
        var query = _context.FundingRequests
            .AsNoTracking()
            .Include(r => r.Country)
            .Include(r => r.LoanType)
            .AsQueryable();

        if (IsStaff)
        {
            if (countryId.HasValue) query = query.Where(r => r.CountryId == countryId);
        }
        else
        {
            // Reps are locked to their own country; the countryId filter is ignored
            query = query.Where(r => r.CountryId == CallerCountryId);
        }

        if (loanTypeId.HasValue) query = query.Where(r => r.LoanTypeId == loanTypeId);
        if (status.HasValue)     query = query.Where(r => r.Status == status);

        var requests = await query
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Ok(requests.Select(ToDto));
    }

    // GET /api/fundingrequests/1/history
    [HttpGet("{id:int}/history")]
    public async Task<ActionResult<IEnumerable<StatusHistoryDto>>> GetHistory(int id)
    {
        // Same visibility rule as GetById: reps only see their own country's requests
        var requestQuery = _context.FundingRequests.Where(r => r.RequestId == id);

        if (!IsStaff)
            requestQuery = requestQuery.Where(r => r.CountryId == CallerCountryId);

        if (!await requestQuery.AnyAsync())
            return NotFound();

        var rows = await _context.RequestStatusHistories
            .AsNoTracking()
            .Where(h => h.RequestId == id)
            .OrderBy(h => h.ChangedAt)
            .ThenBy(h => h.HistoryId)
            .Select(h => new
            {
                h.HistoryId,
                h.FromStatus,
                h.ToStatus,
                ChangedBy = h.ChangedByUser.FullName,
                h.ChangedAt,
                h.Comment
            })
            .ToListAsync();

        var history = rows.Select(h => new StatusHistoryDto(
            h.HistoryId,
            h.FromStatus?.ToString(),
            h.ToStatus.ToString(),
            h.ChangedBy,
            h.ChangedAt,
            h.Comment));

        return Ok(history);
    }

    // POST /api/fundingrequests/1/status
    [HttpPost("{id:int}/status")]
    public async Task<ActionResult<FundingRequestDto>> ChangeStatus(int id, ChangeStatusDto dto)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized();

        var query = _context.FundingRequests
            .Include(r => r.Country)
            .Include(r => r.LoanType)
            .Where(r => r.RequestId == id);

        // Reps can only touch their own country's requests
        if (!IsStaff)
            query = query.Where(r => r.CountryId == CallerCountryId);

        var request = await query.FirstOrDefaultAsync();
        if (request is null)
            return NotFound();

        var from = request.Status;
        var to = dto.NewStatus!.Value;

        if (!IsAllowed(IsStaff, from, to))
            return Conflict(new ProblemDetails { Title = $"Cannot move a request from {from} to {to}." });

        if (to == RequestStatus.Approved)
        {
            if (dto.ApprovedAmountSDR is null || dto.ApprovedAmountSDR <= 0 ||
                dto.ApprovedAmountSDR > request.RequestedAmountSDR)
                return UnprocessableEntity(new ProblemDetails
                {
                    Title = "Approving requires an approved amount greater than 0 and no more than the requested amount."
                });

            request.ApprovedAmountSDR = dto.ApprovedAmountSDR;
        }

        request.Status = to;

        _context.RequestStatusHistories.Add(new RequestStatusHistory
        {
            RequestId = request.RequestId,
            FromStatus = from,
            ToStatus = to,
            ChangedBy = userId,
            Comment = dto.Comment?.Trim()
        });

        // One save writes the status change and its history row in a single transaction
        await _context.SaveChangesAsync();

        return Ok(ToDto(request));
    }

        // PUT /api/fundingrequests/1  (CountryRep only, while Submitted)
    [HttpPut("{id:int}")]
    [Authorize(Roles = "CountryRep")]
    public async Task<ActionResult<FundingRequestDto>> Update(int id, UpdateFundingRequestDto dto)
    {
        var request = await _context.FundingRequests
            .Include(r => r.Country)
            .Include(r => r.LoanType)
            .FirstOrDefaultAsync(r => r.RequestId == id && r.CountryId == CallerCountryId);

        if (request is null)
            return NotFound();

        if (request.Status != RequestStatus.Submitted)
            return Conflict(new ProblemDetails { Title = "A request can only be edited while it is Submitted." });

        var loanType = await _context.LoanTypes
            .FirstOrDefaultAsync(l => l.LoanTypeId == dto.LoanTypeId && l.IsActive);
        if (loanType is null)
            return BadRequest(new ProblemDetails { Title = "Loan type not found." });

        request.LoanTypeId = loanType.LoanTypeId;
        request.LoanType = loanType;
        request.RequestedAmountSDR = dto.RequestedAmountSDR;
        request.Purpose = dto.Purpose.Trim();

        await _context.SaveChangesAsync();

        return Ok(ToDto(request));
    }

    // DELETE /api/fundingrequests/1  (CountryRep only, while Submitted)
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "CountryRep")]
    public async Task<IActionResult> Delete(int id)
    {
        var request = await _context.FundingRequests
            .FirstOrDefaultAsync(r => r.RequestId == id && r.CountryId == CallerCountryId);

        if (request is null)
            return NotFound();

        if (request.Status != RequestStatus.Submitted)
            return Conflict(new ProblemDetails
            {
                Title = "Only a Submitted request can be deleted. Withdraw it instead."
            });

        // A Submitted request has no disbursements, but its history rows must go first
        var history = await _context.RequestStatusHistories
            .Where(h => h.RequestId == id)
            .ToListAsync();

        _context.RequestStatusHistories.RemoveRange(history);
        _context.FundingRequests.Remove(request);

        // One save deletes the history and the request in a single transaction
        await _context.SaveChangesAsync();

        return NoContent();
    }
}