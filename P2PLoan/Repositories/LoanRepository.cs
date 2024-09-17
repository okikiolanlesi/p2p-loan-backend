using System;
using System.Collections.Generic;
using System.Linq;
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


        if (searchParams.LoanRequestId != null)
        {
            query = query.Where(l => l.LoanRequestId == searchParams.LoanRequestId);
        }

        var total = await query.CountAsync();

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

}
