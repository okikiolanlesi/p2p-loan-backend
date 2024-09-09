using System;
using System.Collections;
using System.Collections.Generic;

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
    public IEnumerable<TopUpAccountDetail> TopUpAccountDetails { get; set; }

}
public class TopUpAccountDetail
{
    public string? AccountNumber { get; set; }
    public string? AccountName { get; set; }
    public string? BankCode { get; set; }
    public string? BankName { get; set; }
}
