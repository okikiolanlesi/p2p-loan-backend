using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IWalletTopUpDetailRepository
{
    void Add(WalletTopUpDetail walletTopUpDetail);
    Task<WalletTopUpDetail?> FindById(Guid id);
    Task<WalletTopUpDetail?> FindByAccountNumberAndCode(string AccountNumber, string Code);
    Task<ICollection<WalletTopUpDetail>> GetAllForAWallet(Guid walletId);
    void AddRange(IEnumerable<WalletTopUpDetail> walletTopUpDetails);

    Task<bool> SaveChangesAsync();
}
