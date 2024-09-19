using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.Data;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class LoanRepository : ILoanRepository
{
    private readonly P2PLoanDbContext _context;

    public LoanRepository(P2PLoanDbContext context)
    {
        _context = context;
    }

    public void Add(Loan loan)
    {
        _context.Loans.Add(loan);
    }

    public void AddRange(IEnumerable<Loan> loans)
    {
        _context.Loans.AddRange(loans);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<Loan?> FindById(Guid loanId)
    {
        return await _context.Loans.FirstOrDefaultAsync(x => x.Id == loanId);
    }

    public async Task<PagedResponse<IEnumerable<Loan>>> GetAllAsync(LoanSearchParams searchParams, Guid? userId = null)
    {
        var query = _context.Loans.AsQueryable();

        if (userId != null)
        {
            query = query.Where(l => l.BorrowerId == userId || l.LenderId == userId);
        }

        if (searchParams.LoanOfferId != null)
        {
            query = query.Where(l => l.LoanOfferId == searchParams.LoanOfferId);
        }

        if (searchParams.MinAmount.HasValue)
        {
            query = query.Where(l => l.PrincipalAmount >= searchParams.MinAmount);
        }

        if (searchParams.MaxAmount.HasValue)
        {
            query = query.Where(l => l.PrincipalAmount <= searchParams.MaxAmount);
        }

        if (searchParams.Status.HasValue)
        {
            query = query.Where(l => l.Status == searchParams.Status);
        }

        if (searchParams.LoanRequestId != null)
        {
            query = query.Where(l => l.LoanRequestId == searchParams.LoanRequestId);
        }

        var total = await query.CountAsync();

        // Apply ordering
        if (searchParams.OrderBy != null && searchParams.OrderBy.Any())
        {
            var orderByClauses = searchParams.OrderBy
                .Select(o => $"{o.Field} {o.Direction}")
                .ToArray();
            var orderByString = string.Join(",", orderByClauses);
            query = query.OrderBy(orderByString);
        }

        query = query.Include(l => l.Lender).Include(l => l.Borrower).Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
                     .Take(searchParams.PageSize);

        var loans = await query.ToListAsync();

        return new PagedResponse<IEnumerable<Loan>>
        {
            Items = loans,
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalItems = total
        };
    }

    public async Task<Loan> GetUserActiveLoan(Guid userId)
    {
        return await _context.Loans.FirstOrDefaultAsync(l => l.BorrowerId == userId && l.Status != LoanStatus.Completed);
    }

    public void MarkAsModified(Loan loan)
    {
        _context.Entry(loan).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
    public async Task<List<Loan>> GetLoansDueForAutomaticRepayment()
    {
        var today = DateTime.Now;

        var loansDueForRepayment = await _context.Loans
            .Where(loan => loan.Status == LoanStatus.Active) // Only consider active loans
            .Where(loan => !loan.Defaulted)                  // Exclude defaulted loans
            .Where(loan =>
                _context.Repayments
                    .Where(r => r.LoanId == loan.Id)
                    .OrderByDescending(r => r.CreatedAt)
                    .Select(r => (DateTime?)r.CreatedAt)
                    .FirstOrDefault() == null // If no repayment exists, use loan start date for comparison
                    ? IsLoanDue(today, loan.CreatedAt, loan.RepaymentFrequency)
                    : IsLoanDue(today, _context.Repayments
                                        .Where(r => r.LoanId == loan.Id)
                                        .OrderByDescending(r => r.CreatedAt)
                                        .Select(r => r.CreatedAt)
                                        .First(), loan.RepaymentFrequency)
            )
            .ToListAsync();

        return loansDueForRepayment;
    }

    private bool IsLoanDue(DateTime today, DateTime lastRepaymentDate, PaymentFrequency repaymentFrequency)
    {
        int daysSinceLastRepayment = (today - lastRepaymentDate).Days;

        return repaymentFrequency switch
        {
            PaymentFrequency.daily => daysSinceLastRepayment >= 1,
            PaymentFrequency.weekly => daysSinceLastRepayment >= 7,
            PaymentFrequency.monthly => daysSinceLastRepayment >= 30,
            _ => false
        };
    }


}
