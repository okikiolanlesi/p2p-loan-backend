using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class CreatePinRequestDto
{
    [Required]
    [MaxLength(4)]
    [MinLength(4)]
    public string Pin { get; set; }

    [Required]
    [MaxLength(4)]
    [MinLength(4)]
    public string ConfirmPin { get; set; }
}
