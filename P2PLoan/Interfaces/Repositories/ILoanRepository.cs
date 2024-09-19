using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface ILoanRepository
{
    void Add(Loan loan);
    Task<Loan?> FindById(Guid loanId);
    Task<PagedResponse<IEnumerable<Loan>>> GetAllAsync(LoanSearchParams searchParams, Guid? userId = null);
    Task<Loan> GetUserActiveLoan(Guid userId);
    void AddRange(IEnumerable<Loan> loans);
    void MarkAsModified(Loan loan);
    Task<bool> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task<List<Loan>> GetLoansDueForAutomaticRepayment();

}
