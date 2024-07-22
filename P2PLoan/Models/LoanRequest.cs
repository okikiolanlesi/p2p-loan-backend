using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class LoanRequest
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

        public LoanRequestStatus loanRequestStatus{ get; set; }

        public Type type{ get; set; }

         // foreign key
        public string UserId { get; set; }
        public string LoanOfferId { get; set; }

        public string WalletId { get; set; }

        // navigation properties

        public Wallet Wallet{ get; set; }

    }
}