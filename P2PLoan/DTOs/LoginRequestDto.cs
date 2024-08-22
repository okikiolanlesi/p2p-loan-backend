using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class LoginRequestDto
{
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
}

