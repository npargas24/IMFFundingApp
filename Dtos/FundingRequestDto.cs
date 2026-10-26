namespace ImfFunding.Dtos;

public record FundingRequestDto(
    int RequestId,
    int CountryId,
    string CountryName,
    int LoanTypeId,
    string LoanTypeCode,
    string Status,
    decimal RequestedAmountSDR,
    decimal? ApprovedAmountSDR,
    string Purpose,
    DateTime CreatedAt);