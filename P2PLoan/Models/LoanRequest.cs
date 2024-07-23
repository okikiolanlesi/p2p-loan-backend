using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class LoanRequest:  AuditableEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        public LoanRequestStatus loanRequestStatus{ get; set; }

        public Type type{ get; set; }

         // foreign key
         public Guid UserId { get; set; }
        //public Guid LoanOfferId { get; set; }
        // public Guid WalletId { get; set; }

        // navigation properties
        public User User { get; set; }

        public Wallet Wallet{ get; set; }

    }
}