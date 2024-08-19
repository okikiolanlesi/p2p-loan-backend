using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2PLoan.Models;

public class LoanRequest : AuditableEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid LoanOfferId { get; set; }
    public Guid WalletId { get; set; }
    public LoanRequestStatus Status { get; set; } = LoanRequestStatus.pending;
    public string? AdditionalInformation { get; set; } = string.Empty;
    public DateTime? ProcessingStartTime { get; set; }

    // navigation properties
    public User User { get; set; }
    public LoanOffer LoanOffer { get; set; }
    public Wallet Wallet { get; set; }
}
