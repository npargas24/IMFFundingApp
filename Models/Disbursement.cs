namespace ImfFunding.Models;
public enum DisbursementStatus
{
    Scheduled,
    Released
}
public class Disbursement
{
    public int DisbursementId { get; set; }

    public int RequestId { get; set; }
    public FundingRequest Request { get; set; } = null!;

    public int TrancheNo { get; set; }
    public decimal AmountSDR { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public DateOnly? ReleasedDate { get; set; }   // null until released
    public DisbursementStatus Status { get; set; } = DisbursementStatus.Scheduled;
}