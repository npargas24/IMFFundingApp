using System.ComponentModel.DataAnnotations;
using ImfFunding.Models;

namespace ImfFunding.Dtos;

public class ChangeStatusDto
{
    [Required]
    public RequestStatus? NewStatus { get; set; }

    [MaxLength(500)]
    public string? Comment { get; set; }

    // Only used when moving to Approved
    public decimal? ApprovedAmountSDR { get; set; }
}