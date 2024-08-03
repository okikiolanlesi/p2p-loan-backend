using System;

namespace P2PLoan.Models;

public class CreateLoanOfferRequestDto
{
    public Guid WalletId { get; set; }
    public double Amount { get; set; }
    public PaymentFrequency RepaymentFrequency { get; set; }
    public int GracePeriodDays { get; set; }
    public int LoanDurationDays { get; set; }
    public int AccruingInterestRate { get; set; }
    public int InterestRate { get; set; }
    public string AdditionalInformation { get; set; }

}
