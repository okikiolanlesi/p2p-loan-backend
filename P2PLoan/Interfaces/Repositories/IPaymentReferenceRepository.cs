using System;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IPaymentReferenceRepository
{
    Task<PaymentReference> FindByReferenceAsync(string reference);
    Task<PaymentReference> FindByIdAsync(Guid id);

    void Add(PaymentReference paymentReference);
    void MarkAsModified(PaymentReference paymentReference);
    Task<bool> SaveChangesAsync();

}
