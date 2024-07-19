using System;

namespace P2PLoan;

public class WalletProvider : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
