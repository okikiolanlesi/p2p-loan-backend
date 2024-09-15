using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class ManagedWalletRepository : IManagedWalletRepository
{
    private readonly P2PLoanDbContext context;

    public ManagedWalletRepository(P2PLoanDbContext context)
    {
        this.context = context;
    }
    public void Add(ManagedWallet managedWallet)
    {
        context.ManagedWallets.Add(managedWallet);
    }

    public async Task<ManagedWallet> FindByIdAsync(Guid managedWalletId)
    {
        return await context.ManagedWallets.FirstOrDefaultAsync(x => x.Id == managedWalletId);
    }

    public async Task<IEnumerable<ManagedWallet>> GetWalletsByUserId(Guid userId)
    {
        return await context.ManagedWallets.Where(x => x.UserId == userId).ToListAsync();
    }
    public async Task<ManagedWallet> GetWalletByUserId(Guid userId)
    {
        return await context.ManagedWallets.FirstOrDefaultAsync(x => x.UserId == userId);
    }

    public async Task<ManagedWallet> GetByWalletReferenceAsync(string walletReference)
    {
        return await context.ManagedWallets.FirstOrDefaultAsync(x => x.WalletReference == walletReference);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void MarkAsModified(ManagedWallet managedWallet)
    {
        context.Entry(managedWallet).State = EntityState.Modified;
    }

    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return context.Database.BeginTransactionAsync();
    }
}
