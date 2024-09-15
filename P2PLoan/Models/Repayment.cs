using System;
using P2PLoan.DTOs;

namespace P2PLoan.Models;

public class Repayment : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid LoanId { get; set; }
    public double Amount { get; set; }
    public string? FinancialTransactionId { get; set; }
    public double InterestRate { get; set; }
    public RepaymentStatus Status { get; set; }

    public Loan Loan { get; set; }
}
