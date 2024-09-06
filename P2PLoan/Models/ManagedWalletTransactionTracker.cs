using System;

namespace P2PLoan.Models;

public class ManagedWalletTransactionTracker : AuditableEntity
{
    public Guid Id { get; set; }
    public string InternalReference { get; set; }
    public string ExternalReference { get; set; }
    public string DestinationAccountNumber { get; set; }
    public string DestinationBankCode { get; set; }
    public string SourceAccountNumber { get; set; }
    public string Status { get; set; }
}
