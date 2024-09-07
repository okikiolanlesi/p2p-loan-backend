using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface ILoanRequestRepository
{
    void Add(LoanRequest LoanRequest);
    Task<LoanRequest> FindById(Guid LoanRequestId);
    Task<LoanRequestDto?> FindByIdPublic(Guid LoanRequestId);
    Task<LoanRequest?> FindByIdForAUser(Guid LoanRequestId, Guid UserId);
    Task<PagedResponse<IEnumerable<LoanRequestDto>>> GetAllAsync(LoanRequestSearchParams searchParams, Guid? userId = null);
    void AddRange(IEnumerable<LoanRequest> LoanRequests);
    void MarkAsModified(LoanRequest LoanRequest);
    void Remove(LoanRequest LoanRequest);
    Task<bool> SaveChangesAsync();

    Task<IDbContextTransaction> BeginTransactionAsync();
}
