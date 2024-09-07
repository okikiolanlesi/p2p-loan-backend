using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces;

public interface ILoanRequestService
{
    Task<ServiceResponse<object>> Create(CreateLoanRequestRequestDto createLoanRequestDto);
    Task<ServiceResponse<object>> GetAllForLoggedInUser(LoanRequestSearchParams searchParams);
    Task<ServiceResponse<object>> GetAll(LoanRequestSearchParams searchParams);
    Task<ServiceResponse<object>> GetOne(Guid loanRequestId);
    Task<ServiceResponse<object>> GetOneAdmin(Guid loanRequestId);
    Task<ServiceResponse<object>> Accept(Guid loanRequestId);
    Task<ServiceResponse<object>> Decline(Guid loanRequestId);
    Task<ServiceResponse<object>> Delete(Guid loanRequestId);
    Task<ServiceResponse<object>> DeleteAdmin(Guid loanRequestId);
}
