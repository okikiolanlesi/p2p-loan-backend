using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class Loan: AuditableEntity
    {
        public Guid Id {get; set;}
        public int LoanAmount {get; set;}
        public int Amount_left {get; set;}
        public DateTime DueDate {get; set;}
        public DateTime CreateAt {get; set;} = DateTime.Now;
        public DateTime ModifiedAt {get; set ;} = DateTime.Now;
        public decimal InitialInterestRate {get; set;}
        public LoanStatus loanStatus{get; set;}
        public DateTime Repayment {get; set;} = DateTime.Now;
        public int DurationDay {get; set;}
        public Boolean Defaulted {get; set;}

        public decimal AccruingInterestRate {get; set;}
        public string FinancialTranscationId {get; set;}

        // Navigation properties
        public ICollection<Repayment> Repayments{get; set;}

        
    }
}