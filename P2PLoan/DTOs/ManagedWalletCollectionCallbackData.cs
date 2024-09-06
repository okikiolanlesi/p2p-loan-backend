using System;
using System.Collections.Generic;

namespace P2PLoan.DTOs;

public class ManagedWalletCollectionCallbackData
{
    public Product Product { get; set; }
    public string TransactionReference { get; set; }
    public string PaymentReference { get; set; }
    public DateTime PaidOn { get; set; }
    public string PaymentDescription { get; set; }
    public Dictionary<string, object> MetaData { get; set; }
    public List<PaymentSourceInformation> PaymentSourceInformation { get; set; }
    public DestinationAccountInformation? DestinationAccountInformation { get; set; }
    public double AmountPaid { get; set; }
    public double TotalPayable { get; set; }
    public CardDetails? CardDetails { get; set; }
    public string PaymentMethod { get; set; }
    public string Currency { get; set; }
    public double SettlementAmount { get; set; }
    public string PaymentStatus { get; set; }
    public Customer Customer { get; set; }
}
