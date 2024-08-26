using System;

namespace P2PLoan.Models;

public class PaymentReference
{
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public PaymentReferenceType paymentReferenceType { get; set; }
}
