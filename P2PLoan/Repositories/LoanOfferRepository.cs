using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class LoanOfferRepository : ILoanOfferRepository
{
    public void Add(LoanOffer loanOffer)
    {
        throw new NotImplementedException();
    }

    public void AddRange(IEnumerable<LoanOffer> loanOffers)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LoanOffer>> FindAllByUserId(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<LoanOffer> FindById(Guid loanOfferId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<LoanOffer>> GetAll()
    {
        throw new NotImplementedException();
    }

    public void MarkAsModified(LoanOffer loanOffer)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}
