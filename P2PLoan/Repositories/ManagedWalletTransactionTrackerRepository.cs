using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class ManagedWalletTransactionTrackerRepository : IManagedWalletTransactionTrackerRepository
{
    private readonly P2PLoanDbContext context;

    public ManagedWalletTransactionTrackerRepository(P2PLoanDbContext context)
    {
        this.context = context;
    }
    public void Add(ManagedWalletTransactionTracker managedWalletTransactionTracker)
    {
        context.ManagedWalletTransactionTrackers.Add(managedWalletTransactionTracker);
    }

    public async Task<ManagedWalletTransactionTracker> FindByIdAsync(Guid managedWalletTransactionTrackerId)
    {
        return await context.ManagedWalletTransactionTrackers.FirstOrDefaultAsync(x => x.Id == managedWalletTransactionTrackerId);
    }

    public async Task<ManagedWalletTransactionTracker> FindByInternalReferenceAsync(string internalReference)
    {
        return await context.ManagedWalletTransactionTrackers.FirstOrDefaultAsync(mwtt => mwtt.InternalReference == internalReference);
    }

    public async Task<bool> SaveChangesAsync()
    {

        return await context.SaveChangesAsync() > 0;
    }
}
