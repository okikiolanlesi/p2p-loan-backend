using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class Wallet: AuditableEntity
    {
        public Guid Id { get; set; }
       
        public string WalletAccountNumber { get; set; }
        public DateTime CreatedAt { get; set;} = DateTime.Now;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        // Foreign key property
        public string UserId { get; set; }
        public string WalletProviderId { get; set; }


        
        // Navigation property
        public User User { get; set; }
        

    }
}