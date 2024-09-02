using System;
using System.Collections.Generic;
using P2PLoan.Models;

namespace P2PLoan.DTOs;

public class PublicUserProfileDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool BVNVerified { get; set; }
    public UserType UserType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
}
