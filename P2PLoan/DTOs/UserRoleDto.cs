using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.DTOs
{
    public class UserRoleDto
    {
      public Guid Id { get; set; }
      public Guid UserId { get; set; }
      public Guid RoleId { get; set; }
        //Navigation properties
    public UserRoleUserDto User { get; set; }
    public UserRoleRoleDto Role { get; set; }

    }
}

public class UserRoleUserDto
{

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PinCreated { get; set; }
    public UserType UserType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

     
}

public class UserRoleRoleDto
{

  
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    //navigation properties
    public ICollection<Permission> Permissions { get; set; }
}