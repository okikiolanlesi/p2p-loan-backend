using System;

namespace P2PLoan.DTOs;

public class CreateLoanRequestRequestDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public Guid LoanOfferId { get; set; }
    public Guid WalletId { get; set; }
    public int GracePeriodDays { get; set; }
    public string? AdditionalInformation { get; set; }
}
