using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class WalletRepository : IWalletRepository
{

    private readonly P2PLoanDbContext dbContext;

    public WalletRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(Wallet wallet)
    {
        dbContext.Wallets.Add(wallet);
    }

    public async Task<Wallet?> FindByAccountNumber(string accountNumber)
    {
        return await dbContext.Wallets.FirstOrDefaultAsync(x => x.AccountNumber == accountNumber);
    }

    public async Task<Wallet?> FindById(Guid id)
    {
        return await dbContext.Wallets.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Wallet>> GetAll()
    {
        return await dbContext.Wallets.ToListAsync();
    }

    public async Task<IEnumerable<Wallet>> GetAllForAUser(Guid userId)
    {
        return await dbContext.Wallets.Where(x => x.UserId == userId).ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
