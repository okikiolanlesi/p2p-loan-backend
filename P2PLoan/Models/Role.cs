using System;
using System.Collections.Generic;

namespace P2PLoan.Models;

public class Role : AuditableEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    //navigation properties
    public ICollection<Permission> Permissions { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
