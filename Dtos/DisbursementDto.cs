namespace ImfFunding.Dtos;

public record DisbursementDto(
    int DisbursementId,
    int TrancheNo,
    decimal AmountSDR,
    DateOnly ScheduledDate,
    DateOnly? ReleasedDate,
    string Status);