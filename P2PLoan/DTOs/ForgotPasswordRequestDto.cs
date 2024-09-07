using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class ForgotPasswordRequestDto
{
    [Required]
    public string Email { get; set; }
}