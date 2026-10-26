using System.ComponentModel.DataAnnotations;

namespace ImfFunding.Dtos;

public class CreateDisbursementDto
{
    [Required, Range(typeof(decimal), "0.01", "1000000000000")]
    public decimal AmountSDR { get; set; }

    [Required]
    public DateOnly? ScheduledDate { get; set; }
}