using System;
using System.Collections.Generic;
using P2PLoan.Models;

namespace P2PLoan.DTOs;

public class UserDto
{

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool PinCreated { get; set; }
    public UserType UserType { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    //Navigation properties
    public ICollection<UserRole> UserRoles { get; set; }
}

