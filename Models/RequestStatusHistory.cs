namespace ImfFunding.Models;

public class RequestStatusHistory
{
    public long HistoryId { get; set; }

    public int RequestId { get; set; }
    public FundingRequest Request { get; set; } = null!;

    public RequestStatus? FromStatus { get; set; }   // null on the first row
    public RequestStatus ToStatus { get; set; }

    public int ChangedBy { get; set; }
    public ApplicationUser ChangedByUser { get; set; } = null!;

    public DateTime ChangedAt { get; set; }
    public string? Comment { get; set; }
}