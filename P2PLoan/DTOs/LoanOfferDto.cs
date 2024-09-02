using System;
using P2PLoan.DTOs;

namespace P2PLoan.Models;

public class LoanOfferDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public double Amount { get; set; }
    public PaymentFrequency RepaymentFrequency { get; set; }
    public int GracePeriodDays { get; set; }
    public int LoanDurationDays { get; set; }
    public double AccruingInterestRate { get; set; }
    public double InterestRate { get; set; }
    public string AdditionalInformation { get; set; }
    public LoanOfferType Type { get; set; }
    public bool Active { get; set; }
    public PublicUserProfileDto User { get; set; }
    public WalletDto Wallet { get; set; }
}
