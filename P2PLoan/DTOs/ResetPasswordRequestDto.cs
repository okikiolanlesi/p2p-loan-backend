using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class ResetPasswordRequestDto
{
    [Required]
    public string Email { get; set; }
    [Required]
    public string Token { get; set; }
    [Required]
    public string NewPassword { get; set; }

}
