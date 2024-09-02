using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class ResendVerificationEmailDto
{
    [Required]
    public string Email { get; set; }
}
