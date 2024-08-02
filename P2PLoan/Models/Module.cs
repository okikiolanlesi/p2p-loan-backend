using System;
using P2PLoan.Constants;

namespace P2PLoan.Models;

public class Module : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Modules Identifier { get; set; }
}
