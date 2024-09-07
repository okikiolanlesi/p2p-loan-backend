using System;
using P2PLoan.Models;

namespace P2PLoan.DTOs;

public class LoanRequestDto
{

    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid LoanOfferId { get; set; }
    public Guid WalletId { get; set; }
    public LoanRequestStatus Status { get; set; }
    public string? AdditionalInformation { get; set; } = string.Empty;
    public DateTime? ProcessingStartTime { get; set; }

    // navigation properties
    public PublicUserProfileDto User { get; set; }
    public LoanOfferDto LoanOffer { get; set; }
    public WalletDto Wallet { get; set; }
}
