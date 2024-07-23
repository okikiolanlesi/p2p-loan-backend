using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class Transaction: AuditableEntity
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.UtcNow;
        public int BeginingBalance { get; set; }
        public TransactionStatus transactionStatus{ get; set; }
        public int EndingBalance { get; set; }
        public Type type{ get; set; }
        public string FinancialTranscationId { get; set; }

        //foreign key
         public Guid UserId { get; set; }

         public User User{ get; set; }

        
    }
}