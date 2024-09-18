using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Interfaces;
using P2PLoan.Data;
using P2PLoan.DTOs;
using P2PLoan.DTOs.SearchParams;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class RepaymentRepository : IRepaymentRepository
{
    private readonly P2PLoanDbContext context;

    public RepaymentRepository(P2PLoanDbContext context)
    {
        this.context = context;
    }
    public void Add(Repayment repayment)
    {
        context.Repayments.Add(repayment);
    }

    public void AddRange(IEnumerable<Repayment> repayments)
    {
        context.Repayments.AddRange(repayments);
    }

    public async Task<Repayment> FindById(Guid repaymentId)
    {
        return await context.Repayments.FirstOrDefaultAsync(x => x.Id == repaymentId);
    }

    public async Task<PagedResponse<IEnumerable<Repayment>>> GetAllAsync(RepaymentSearchParams searchParams)
    {
        var query = context.Repayments.AsQueryable();

        if (searchParams.LoanId != null)
        {
            query = query.Where(l => l.LoanId == searchParams.LoanId);
        }

        if (searchParams.MinAmount != null)
        {
            query = query.Where(l => l.Amount >= searchParams.MinAmount);
        }

        if (searchParams.MaxAmount != null)
        {
            query = query.Where(l => l.Amount <= searchParams.MaxAmount);
        }

        var total = await query.CountAsync();

        query = query.OrderByDescending(r => r.CreatedAt).Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
                     .Take(searchParams.PageSize);

        return new PagedResponse<IEnumerable<Repayment>>
        {
            Items = await query.ToListAsync(),
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize,
            TotalItems = total
        };
    }

    public void MarkAsModified(Repayment repayment)
    {
        context.Entry(repayment).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }
    public async Task<List<Repayment>> GetPendingRepaymentsForALoan(Guid loanId)
    {
        return await context.Repayments.Where(r => r.Status == RepaymentStatus.pending && r.LoanId == loanId).ToListAsync(); ;
    }
}
