using System;

namespace P2PLoan.DTOs;

public class CreateWalletDto
{
    public string WalletReference { get; set; }
    public string WalletName { get; set; }
    public string CustomerName { get; set; }
    public BVNDetails BvnDetails { get; set; }
    public string CustomerEmail { get; set; }
}

public class BVNDetails
{
    public string Bvn { get; set; }
    public string BvnDateOfBirth { get; set; }
}