using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2PLoan.Models;

public class LoanOffer : AuditableEntity
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

    // navigation properties
    public User User { get; set; }
    public Wallet Wallet { get; set; }
}
