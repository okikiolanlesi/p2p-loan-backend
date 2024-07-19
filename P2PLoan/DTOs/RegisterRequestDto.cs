using System;

namespace P2PLoan.DTOs;

public class RegisterRequestDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string PIN { get; set; }
    public string BVN { get; set; }
    public string Role { get; set; }
}
