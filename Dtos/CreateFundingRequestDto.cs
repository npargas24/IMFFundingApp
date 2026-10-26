using System.ComponentModel.DataAnnotations;

namespace ImfFunding.Dtos;

public class CreateFundingRequestDto
{
    [Required, Range(1, int.MaxValue)]
    public int LoanTypeId { get; set; }

    [Required, Range(typeof(decimal), "0.01", "1000000000000")]
    public decimal RequestedAmountSDR { get; set; }

    [Required, MaxLength(1000)]
    public string Purpose { get; set; } = string.Empty;
}