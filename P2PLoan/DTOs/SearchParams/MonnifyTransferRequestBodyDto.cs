using System;

namespace P2PLoan.DTOs;

public class MonnifyTransferRequestBodyDto
{
    public double Amount { get; set; }
    public string Reference { get; set; }
    public string Narration { get; set; } = string.Empty;
    public string DestinationBankCode { get; set; }
    public string DestinationAccountNumber { get; set; }
    public string Currency { get; set; } = "NGN";
    public string SourceAccountNumber { get; set; }
    public bool Async { get; set; }
}
