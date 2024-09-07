using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface ILoanOfferRepository
{
    void Add(LoanOffer loanOffer);
    Task<LoanOffer?> FindById(Guid loanOfferId);
    Task<PagedResponse<IEnumerable<LoanOfferDto>>> GetAllAsync(LoanOfferSearchParams searchParams, Guid? userId = null);
    void AddRange(IEnumerable<LoanOffer> loanOffers);
    void MarkAsModified(LoanOffer loanOffer);
    Task<bool> SaveChangesAsync();
}
