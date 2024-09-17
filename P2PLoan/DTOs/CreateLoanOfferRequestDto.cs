using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.Models;

public class CreateLoanOfferRequestDto
{
    [Required]
    public Guid WalletId { get; set; }
    [Required]
    public double Amount { get; set; }
    [Required]
    public PaymentFrequency RepaymentFrequency { get; set; }
    [Required]
    public int GracePeriodDays { get; set; }
    [Required]
    public int LoanDurationDays { get; set; }
    [Required]
    public double AccruingInterestRate { get; set; }
    [Required]
    public double InterestRate { get; set; }
    [Required]
    public string AdditionalInformation { get; set; }

}
