using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class CreateLoanRequestRequestDto
{
    [Required]
    public Guid LoanOfferId { get; set; }
    [Required]
    public Guid WalletId { get; set; }
    [Required]
    public string? AdditionalInformation { get; set; }
}
