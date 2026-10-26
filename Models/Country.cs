using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ImfFunding.Models;

[Index(nameof(IsoCode), IsUnique = true)]
public class Country
{
    [Key]
    public int CountryId { get; set; }

    [Required, MaxLength(3)]
    public string IsoCode { get; set; } = string.Empty;

    [Required, MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? Region { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; }
}