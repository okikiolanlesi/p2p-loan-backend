using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P2PLoan.Interfaces;

public interface IWalletRepository
{
    void Add(Wallet wallet);
    Task<Wallet?> FindById(Guid id);
    Task<IEnumerable<Wallet>> GetAll();
    Task<Wallet?> FindByAccountNumber(string accountNumber);
    Task<IEnumerable<Wallet>> GetAllForAUser(Guid userId);
    Task<bool> SaveChangesAsync();


}
