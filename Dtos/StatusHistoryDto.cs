namespace ImfFunding.Dtos;

public record StatusHistoryDto(
    long HistoryId,
    string? FromStatus,
    string ToStatus,
    string ChangedBy,
    DateTime ChangedAt,
    string? Comment);