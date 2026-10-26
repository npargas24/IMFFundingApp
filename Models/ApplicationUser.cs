using System.ComponentModel.DataAnnotations;

namespace ImfFunding.Models;

public enum UserRole
{
    CountryRep,
    IMFStaff
}
public class ApplicationUser
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public int? CountryId { get; set; }       // null for IMFStaff
    public Country? Country { get; set; }    // navigation property
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}