using System;

namespace P2PLoan.Models;

public class ManagedWalletTransaction : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ManagedWalletId { get; set; }
    public double Amount { get; set; }
    public double Fee { get; set; } = 0;
    public string Narration { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
    public string TransactionReference { get; set; }


    public bool IsCredit { get; set; }

    //navigation properties
    public ManagedWallet ManagedWallet { get; set; }

}

