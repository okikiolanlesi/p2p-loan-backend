using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class LoanOffer: AuditableEntity
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public Frequency frequency{ get; set; }
        //type
        //field
        public int GracePeriod { get; set; }
        public int Duration { get; set; }
        public decimal AccruingInterest { get; set; }
        public string AdditionalInformation { get; set; }

        public Type type{ get; set; }
        public DateTime CreatedAt { get; set; }= DateTime.Now;
        public DateTime ModifiedAt { get; set; }= DateTime.Now;
        public string DisbursementWalletId { get; set; }


        // foreign key
        public string UserId { get; set; }

        //Navigation properties
        public ICollection<LoanRequest> LoanRequests { get; set; }

    }
}