using System;
using System.ComponentModel.DataAnnotations;

namespace P2PLoan.DTOs;

public class CreateWalletDto
{
    [Required]
    public string WalletReference { get; set; }
    [Required]
    public string WalletName { get; set; }
    [Required]
    public string CustomerName { get; set; }
    [Required]
    public BVNDetails BvnDetails { get; set; }
    [Required]
    public string CustomerEmail { get; set; }
}

public class BVNDetails
{
    [Required]
    public string Bvn { get; set; }
    [Required]
    public string BvnDateOfBirth { get; set; }
}