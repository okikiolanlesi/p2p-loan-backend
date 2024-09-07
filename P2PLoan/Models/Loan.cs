using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace P2PLoan.Models;

public class Loan : AuditableEntity
{
    public Guid Id { get; set; }
    [ForeignKey("Borrower")]
    public Guid BorrowerId { get; set; }
    [ForeignKey("Lender")]
    public Guid LenderId { get; set; }
    public Guid LoanOfferId { get; set; }
    public Guid LoanRequestId { get; set; }
    public double AmountLeft { get; set; }
    public DateTime DueDate { get; set; }
    public double InitialInterestRate { get; set; }
    public LoanStatus Status { get; set; }
    public PaymentFrequency RepaymentFrequency { get; set; }
    public int LoanDurationDays { get; set; }
    public bool Defaulted { get; set; }
    public double PrincipalAmount { get; set; }
    public double AccruingInterestRate { get; set; }
    public string FinancialTransactionId { get; set; }

    // navigation properties
    public User Borrower { get; set; }
    public User Lender { get; set; }
    public LoanOffer LoanOffer { get; set; }
    public LoanRequest LoanRequest { get; set; }
}
