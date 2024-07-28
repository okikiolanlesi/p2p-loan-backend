using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P2PLoan.Interfaces;

public interface IWalletProviderRepository
{
    void Add(WalletProvider walletProvider);
    Task<WalletProvider?> FindById(Guid id);
    Task<IEnumerable<WalletProvider>> GetAll();
    Task<WalletProvider?> FindBySlug(string slug);
    Task<bool> SaveChangesAsync();
}
