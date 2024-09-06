using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IManagedWalletRepository
{
    void Add(ManagedWallet managedWallet);
    Task<ManagedWallet> GetByWalletReferenceAsync(string walletReference);
    Task<ManagedWallet> FindByIdAsync(Guid managedWalletId);
    Task<IEnumerable<ManagedWallet>> GetWalletsByUserId(Guid userId);
    Task<ManagedWallet> GetWalletByUserId(Guid userId);
    void MarkAsModified(ManagedWallet managedWallet);
    Task<bool> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
