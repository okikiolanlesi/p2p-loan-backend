using System;
using P2PLoan.Models;

namespace P2PLoan.Models;

public class Wallet : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid WalletProviderId { get; set; }
    public string AccountNumber { get; set; }
    public string ReferenceId { get; set; }

    // navigation properties
    public User User { get; set; }
    public WalletProvider WalletProvider { get; set; }
}
