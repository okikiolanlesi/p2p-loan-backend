using System;

namespace P2PLoan.DTOs;

public class MonnifyDisbursementCallbackData
{
    public double Amount { get; set; }
    public string TransactionReference { get; set; }
    public double Fee { get; set; }
    public string TransactionDescription { get; set; }
    public string DestinationAccountNumber { get; set; }
    public string SessionId { get; set; }
    public DateTime CreatedOn { get; set; }
    public string DestinationAccountName { get; set; }
    public string Reference { get; set; }
    public string DestinationBankCode { get; set; }
    public DateTime CompletedOn { get; set; }
    public string Narration { get; set; }
    public string Currency { get; set; }
    public string DestinationBankName { get; set; }
}
