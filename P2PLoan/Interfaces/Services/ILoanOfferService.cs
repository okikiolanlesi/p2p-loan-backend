using System;
using System.Threading.Tasks;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface ILoanOfferService
{
    Task<ServiceResponse<object>> Create(CreateLoanOfferRequestDto createLoanOfferRequestDto);
    Task<ServiceResponse<object>> GetAllForLoggedInUser(LoanOfferSearchParams searchParams);
    Task<ServiceResponse<object>> GetAll(LoanOfferSearchParams searchParams);
    Task<ServiceResponse<object>> GetOne(Guid loanOfferId);
    Task<ServiceResponse<object>> DisableLoanOffer(Guid loanOfferId);
}
