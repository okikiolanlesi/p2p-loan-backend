using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2PLoan.Models;

public class LoanRequest : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid LoanOfferId { get; set; }
    public Guid WalletId { get; set; }
    public int Amount { get; set; }
    public LoanRequestStatus Status { get; set; }
    public int GracePeriodDays { get; set; }
    public int LoanDurationDays { get; set; }
    public int AccruingInterestRate { get; set; }
    public int InterestRate { get; set; }
    public string AdditionalInformation { get; set; }

    // navigation properties
    public User User { get; set; }
    public LoanOffer LoanOffer { get; set; }
    public Wallet Wallet { get; set; }
}
