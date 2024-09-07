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

    public void Add(Loan Loan)
    {
        _context.Loans.Add(Loan);
    }

    public void AddRange(IEnumerable<Loan> Loans)
    {
        _context.Loans.AddRange(Loans);
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return _context.Database.BeginTransactionAsync();
    }

    public async Task<Loan?> FindById(Guid LoanId)
    {
        return await _context.Loans.FindAsync(LoanId);
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

        if (searchParams.MinAmount != null)
        {
            query = query.Where(l => l.PrincipalAmount >= searchParams.MaxAmount);
        }

        if (searchParams.MaxAmount != null)
        {
            query = query.Where(l => l.PrincipalAmount <= searchParams.MaxAmount);
        }


        if (searchParams.LoanRequestId != null)
        {
            query = query.Where(l => l.LoanRequestId == searchParams.LoanRequestId);
        }

        var total = await query.CountAsync();

        query = query.Skip(searchParams.PageNumber * searchParams.PageSize)
                     .Take(searchParams.PageSize);

        return new PagedResponse<IEnumerable<Loan>>
        {
            Items = await query.ToListAsync(),
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalItems = total
        };
    }

    public Task<Loan> GetUserActiveLoan(Guid userId)
    {
        return _context.Loans.FirstOrDefaultAsync(l => l.BorrowerId == userId && l.Status != LoanStatus.Completed);
    }

    public void MarkAsModified(Loan Loan)
    {
        _context.Entry(Loan).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }

}
