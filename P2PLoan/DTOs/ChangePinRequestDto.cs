using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class ChangePinRequestDto
{
    [Required]
    [MaxLength(4)]
    [MinLength(4)]
    public string OldPin { get; set; }

    [Required]
    [MaxLength(4)]
    [MinLength(4)]
    public string NewPin { get; set; }

    [Required]
    [MaxLength(4)]
    [MinLength(4)]
    public string ConfirmNewPin { get; set; }
}
