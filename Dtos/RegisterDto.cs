using System.ComponentModel.DataAnnotations;

namespace ImfFunding.Dtos;

//a DTO should contain only the fields the client is allowed to control.
public class RegisterDto
{
    [Required, EmailAddress, MaxLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8), MaxLength(100)]
    public string Password { get; set; } = string.Empty;

    [Required, MaxLength(150)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public int CountryId { get; set; }
}