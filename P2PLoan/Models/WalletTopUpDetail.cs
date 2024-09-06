using System;

namespace P2PLoan.Models;

public class WalletTopUpDetail : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public string BankCode { get; set; }
    public string BankName { get; set; }

}
