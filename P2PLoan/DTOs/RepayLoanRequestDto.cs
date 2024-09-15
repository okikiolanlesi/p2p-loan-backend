using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class RepayLoanRequestDto
{
    [Required]
    public double Amount { get; set; }
    [Required]
    public Guid LoanId { get; set; }
    [Required]
    public string? PIN { get; set; }

}
