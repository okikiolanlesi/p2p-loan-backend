using System;

namespace P2PLoan.Models;

public class UserRole : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    //Navigation properties
    public User User { get; set; }
    public Role Role { get; set; }
}




