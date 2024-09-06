using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IManagedWalletTransactionRepository
{
    void Add(ManagedWalletTransaction managedWalletTransaction);
    Task<ManagedWalletTransaction?> FindByIdAsync(Guid managedWalletTransactionId);
    Task<PagedResponse<IEnumerable<ManagedWalletTransaction>>> GetTransactionsByWalletId(Guid managedWalletId, int page, int pageSize);
    void MarkAsModified(ManagedWalletTransaction managedWalletTransaction);
    Task<bool> SaveChangesAsync();
}
