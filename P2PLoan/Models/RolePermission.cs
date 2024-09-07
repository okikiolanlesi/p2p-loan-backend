using System;

namespace P2PLoan.Models;

public class RolePermission : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public Guid PermissionId { get; set; }

    //Navigation properties
    public Role Role { get; set; }
    public Permission Permission { get; set; }
}
