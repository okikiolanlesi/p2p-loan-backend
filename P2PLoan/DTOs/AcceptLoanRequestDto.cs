using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class AcceptLoanRequestDto
{
    [Required]
    public string PIN { get; set; }
}
