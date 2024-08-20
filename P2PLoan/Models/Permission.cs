using System;

namespace P2PLoan.Models;

public class Permission : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid ModuleId { get; set; }
    public PermissionAction Action { get; set; }
    //Navigation properties
    public Module Module { get; set; }
}
