using System;
using System.Collections.Generic;

namespace P2PLoan.Models;

public class User
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    //Navigation properties
    public ICollection<UserRole> UserRoles { get; set; }
    public ICollection<LoanOffer> LoanOffers { get; set; }
    public ICollection<Wallet> Wallets{ get; set; }
    public ICollection<LoanRequest> LoanRequests{ get; set; }
    public ICollection<Transaction> Transactions{ get; set; }

    public ICollection<Repayment> Repayments{ get; set; }

    public ICollection<Notification> Notifications{ get; set; }
}
