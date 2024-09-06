using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IWalletProviderRepository
{
    void Add(WalletProvider walletProvider);
    void AddRange(List<WalletProvider> walletProviders);
    Task<WalletProvider?> FindById(Guid id);
    Task<IEnumerable<WalletProvider>> GetAll();
    Task<WalletProvider?> FindBySlug(WalletProviders slug);
    void MarkAsModified(WalletProvider walletProvider);
    Task<bool> SaveChangesAsync();
}
