using System;

namespace P2PLoan.DTOs;

public class CreateWalletResponseDto
{
    public string? AccountNumber { get; set; }
    public string? WalletReference { get; set; }
    public string? AccountName { get; set; }
    public string? FeeBearer { get; set; }
    public string? BVN { get; set; }
    public string? BVNDateOfBirth { get; set; }
    public string? CustomerEmail { get; set; }
    public string? CustomerName { get; set; }
}
