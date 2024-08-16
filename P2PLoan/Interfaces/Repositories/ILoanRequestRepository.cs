using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface ILoanRequestRepository
{
    void Add(LoanRequest LoanRequest);
    Task<LoanRequest?> FindById(Guid LoanRequestId);
    Task<PagedResponse<IEnumerable<LoanRequest>>> GetAllAsync(LoanRequestSearchParams searchParams, Guid? userId = null);
    void AddRange(IEnumerable<LoanRequest> LoanRequests);
    void MarkAsModified(LoanRequest LoanRequest);
    void Remove(LoanRequest LoanRequest);
    Task<bool> SaveChangesAsync();
}
