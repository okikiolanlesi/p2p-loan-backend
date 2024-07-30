using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;

namespace P2PLoan.Repositories;

public class WalletProviderRepository : IWalletProviderRepository
{
    private readonly P2PLoanDbContext dbContext;

    public WalletProviderRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(WalletProvider walletProvider)
    {
        dbContext.WalletProviders.Add(walletProvider);
    }

    public void AddRange(List<WalletProvider> walletProviders)
    {
        dbContext.WalletProviders.AddRange(walletProviders);
    }

    public async Task<WalletProvider?> FindById(Guid id)
    {
        return await dbContext.WalletProviders.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<WalletProvider?> FindBySlug(WalletProviders slug)
    {
        return await dbContext.WalletProviders.FirstOrDefaultAsync(x => x.Slug == slug);
    }

    public async Task<IEnumerable<WalletProvider>> GetAll()
    {
        return await dbContext.WalletProviders.ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
