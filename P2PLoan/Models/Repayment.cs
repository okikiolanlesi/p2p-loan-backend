using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class Repayment:AuditableEntity
    {
        
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ModifiedAt { get; set;} = DateTime.UtcNow;
        public string FinancialTranscationId { get; set; }

        public decimal PrincipalAmount { get; set; }
        public decimal InterestRate { get; set; }

        //Navigation Properties
        public User User {get; set;}
       // public Guid UserId { get; set; }
        public Loan Loan{ get; set; }
        public Guid LoanId { get; set; }


    }
}