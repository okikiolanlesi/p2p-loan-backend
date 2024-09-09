using System;
using System.Collections.Generic;

namespace P2PLoan.Models;

public class ManagedWallet : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double AvailableBalance { get; set; }
    public double LedgerBalance { get; set; }
    public string WalletReference { get; set; }
    public string AccountName { get; set; }

    //navigation properties
    public User User { get; set; }
    public ICollection<ManagedWalletTransaction> WalletTransactions { get; set; }
}
