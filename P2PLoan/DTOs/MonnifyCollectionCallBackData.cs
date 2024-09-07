using System;
using System.Collections.Generic;

namespace P2PLoan.DTOs;

public class MonnifyCollectionCallBackData
{
    public Product Product { get; set; }
    public string TransactionReference { get; set; }
    public string PaymentReference { get; set; }
    public DateTime PaidOn { get; set; }
    public string PaymentDescription { get; set; }
    public Dictionary<string, object> MetaData { get; set; }
    public List<PaymentSourceInformation> PaymentSourceInformation { get; set; }
    public DestinationAccountInformation DestinationAccountInformation { get; set; }
    public double AmountPaid { get; set; }
    public double TotalPayable { get; set; }
    public CardDetails CardDetails { get; set; }
    public string PaymentMethod { get; set; }
    public string Currency { get; set; }
    public double SettlementAmount { get; set; }
    public string PaymentStatus { get; set; }
    public Customer Customer { get; set; }
}

public class Product
{
    public string Reference { get; set; }
    public string Type { get; set; }
}

public class MetaData
{
    public string Name { get; set; }
    public string Age { get; set; }
}

public class PaymentSourceInformation
{
    public string BankCode { get; set; }
    public decimal AmountPaid { get; set; }
    public string AccountName { get; set; }
    public string SessionId { get; set; }
    public string AccountNumber { get; set; }
}

public class DestinationAccountInformation
{
    public string BankCode { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
}

public class CardDetails
{
    public string Last4 { get; set; }
    public string ExpMonth { get; set; }
    public string ExpYear { get; set; }
    public string Bin { get; set; }
    public bool Reusable { get; set; }
}

public class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
}
