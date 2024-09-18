using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class ManagedWalletTransactionRepository : IManagedWalletTransactionRepository
{
    private readonly P2PLoanDbContext dbContext;

    public ManagedWalletTransactionRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(ManagedWalletTransaction managedWalletTransaction)
    {
        dbContext.ManagedWalletTransactions.Add(managedWalletTransaction);
    }

    public async Task<ManagedWalletTransaction?> FindByIdAsync(Guid managedWalletTransactionId)
    {
        return await dbContext.ManagedWalletTransactions.FirstOrDefaultAsync(mwt => mwt.Id == managedWalletTransactionId);
    }

    public async Task<PagedResponse<IEnumerable<ManagedWalletTransaction>>> GetTransactionsByWalletId(Guid managedWalletId, int page, int pageSize)
    {
        IQueryable<ManagedWalletTransaction> query = dbContext.ManagedWalletTransactions.Where(mwt => mwt.ManagedWalletId == managedWalletId);

        // Apply pagination
        var totalItems = query.Count();
        var items = await query.OrderByDescending(mwt => mwt.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<IEnumerable<ManagedWalletTransaction>>
        {
            Items = items,
            TotalItems = totalItems,
            PageNumber = page,
            PageSize = pageSize
        };
    }

    public void MarkAsModified(ManagedWalletTransaction managedWalletTransaction)
    {
        dbContext.Entry(managedWalletTransaction).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
