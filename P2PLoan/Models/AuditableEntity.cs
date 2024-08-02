using System;
using System.ComponentModel.DataAnnotations.Schema;
using P2PLoan.Models;

public abstract class AuditableEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    [ForeignKey("CreatedBy")]
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }

    [ForeignKey("ModifiedBy")]
    public Guid ModifiedById { get; set; }
    public User ModifiedBy { get; set; }
}