using System;
using System.ComponentModel.DataAnnotations;
using P2PLoan.Models;

namespace P2PLoan.DTOs;

public class RegisterRequestDto
{
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [MinLength(6)]
    public string Password { get; set; }
    [Required]
    [MinLength(11)]
    [MaxLength(13)]
    public string PhoneNumber { get; set; }
    [Required]
    [MinLength(11)]
    [MaxLength(11)]
    public string BVN { get; set; }
    [Required]
    [MinLength(11)]
    [MaxLength(11)]
    public string NIN { get; set; }
    [Required]
    public UserType UserType { get; set; }
    [Required]
    public string BvnDateOfBirth { get; set; }
    [Required]
    public Guid WalletProviderId { get; set; }
}
