using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan;

public class ChangePasswordRequestDto
{
    [Required]
    [MinLength(6)]
    public string OldPassword { get; set; }

    [Required]
    [MinLength(6)]
    public string NewPassword { get; set; }

    [Required]
    [MinLength(6)]
    public string ConfirmNewPassword { get; set; }
}
