using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class WalletTopUpDetailRepository : IWalletTopUpDetailRepository
{
    private readonly P2PLoanDbContext dbContext;

    public WalletTopUpDetailRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(WalletTopUpDetail walletTopUpDetail)
    {
        dbContext.WalletTopUpDetails.Add(walletTopUpDetail);
    }

    public void AddRange(IEnumerable<WalletTopUpDetail> walletTopUpDetails)
    {
        dbContext.WalletTopUpDetails.AddRange(walletTopUpDetails);
    }

    public async Task<WalletTopUpDetail> FindByAccountNumberAndCode(string accountNumber, string code)
    {
        return await dbContext.WalletTopUpDetails.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber && x.BankCode == code);
    }

    public async Task<WalletTopUpDetail> FindById(Guid id)
    {
        return await dbContext.WalletTopUpDetails.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ICollection<WalletTopUpDetail>> GetAllForAWallet(Guid walletId)
    {
        return await dbContext.WalletTopUpDetails.Where(x => x.WalletId == walletId).ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
