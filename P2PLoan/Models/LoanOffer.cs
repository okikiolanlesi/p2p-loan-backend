﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2PLoan.Models;

public class LoanOffer : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletId { get; set; }
    public int Amount { get; set; }
    public PaymentFrequency RepaymentFrequency { get; set; }
    public int GracePeriodDays { get; set; }
    public int LoanDurationDays { get; set; }
    public int AccruingInterestRate { get; set; }
    public int InterestRate { get; set; }
    public string AdditionalInformation { get; set; }
    public LoanOfferType Type { get; set; }

    // navigation properties
    public User User { get; set; }
    public Wallet Wallet { get; set; }
}
