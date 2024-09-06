using System;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IManagedWalletTransactionTrackerRepository
{
    void Add(ManagedWalletTransactionTracker managedWalletTransactionTracker);
    Task<ManagedWalletTransactionTracker?> FindByIdAsync(Guid managedWalletTransactionTrackerId);
    Task<ManagedWalletTransactionTracker?> FindByInternalReferenceAsync(string internalReference);
    Task<bool> SaveChangesAsync();
}
