using System;

namespace P2PLoan.Models;

public class PaymentReference : AuditableEntity
{
    public Guid Id { get; set; }
    public string Reference { get; set; }
    public Guid ResourceId { get; set; }
    public PaymentReferenceType paymentReferenceType { get; set; }
}
