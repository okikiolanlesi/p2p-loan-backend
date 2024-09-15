using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces;

public interface ILoanService
{
    Task<ServiceResponse<object>> GetMyLoans(LoanSearchParams loanSearchParams);
    Task<ServiceResponse<object>> GetActiveLoan();
    Task<ServiceResponse<object>> GetALoan(Guid loanId);
    Task<ServiceResponse<object>> GetLoanRepayments(Guid loanId, RepaymentSearchParams repaymentSearchParams);
    Task<ServiceResponse<object>> GetALoanRepayment(Guid repaymentId);
    Task<ServiceResponse<object>> RepayLoan(RepayLoanRequestDto repayLoanRequestDto);
}
