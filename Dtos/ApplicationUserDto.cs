namespace ImfFunding.Dtos;

public record ApplicationUserDto(int UserId, string Email, string FullName, string Role, int? CountryId);