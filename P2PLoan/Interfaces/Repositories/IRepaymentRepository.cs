using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IRepaymentRepository
{
    void Add(Repayment repayment);
    Task<Repayment?> FindById(Guid repaymentId);
    Task<PagedResponse<IEnumerable<Repayment>>> GetAllAsync(RepaymentSearchParams searchParams);
    void AddRange(IEnumerable<Repayment> repayments);
    void MarkAsModified(Repayment repayment);
    Task<bool> SaveChangesAsync();
    Task<List<Repayment>> GetPendingRepaymentsForALoan(Guid loanId);
}
