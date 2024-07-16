using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2PLoan.Models;

public class Module : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
