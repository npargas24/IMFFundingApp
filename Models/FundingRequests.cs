namespace ImfFunding.Models;

public class FundingRequest
{
    public int RequestId { get; set; }

    public int CountryId { get; set; }
    public Country Country { get; set; } = null!;

    public int LoanTypeId { get; set; }
    public LoanType LoanType { get; set; } = null!;

    public RequestStatus Status { get; set; } = RequestStatus.Submitted;

    public decimal RequestedAmountSDR { get; set; }
    public decimal? ApprovedAmountSDR { get; set; }   // set when the request is Approved
    public string Purpose { get; set; } = string.Empty;

    public int CreatedBy { get; set; }
    public ApplicationUser Creator { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}