using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class PaymentReferenceRepository : IPaymentReferenceRepository
{
    private readonly P2PLoanDbContext context;

    public PaymentReferenceRepository(P2PLoanDbContext context)
    {
        this.context = context;
    }
    public void Add(PaymentReference paymentReference)
    {
        context.PaymentReferences.Add(paymentReference);
    }

    public async Task<PaymentReference> FindByIdAsync(Guid id)
    {
        return await context.PaymentReferences.FirstOrDefaultAsync(pr => pr.Id == id);
    }

    public async Task<PaymentReference> FindByReferenceAsync(string reference)
    {
        return await context.PaymentReferences.FirstOrDefaultAsync(pr => pr.Reference == reference);
    }

    public void MarkAsModified(PaymentReference paymentReference)
    {
        context.Entry(paymentReference).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
}
