using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class WithdrawRequestDto
{
    [Required]
    public double Amount { get; set; }
    [Required]
    public Guid WalletId { get; set; }
    [Required]
    public string DestinationBankCode { get; set; }
    [Required]
    public string DestinationAccountNumber { get; set; }
    [Required]
    public string Currency { get; set; } = "NGN";
}
