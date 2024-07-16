using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.Data;
using P2PLoan.Interfaces;

namespace P2PLoan.Repositories;

public class SeedRepository : ISeedRepository
{
    private readonly P2PLoanDbContext dbContext;

    public SeedRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(Seed seed)
    {
        dbContext.Seeds.Add(seed);
    }

    public void AddRange(IEnumerable<Seed> seeds)
    {
        dbContext.Seeds.AddRange(seeds);
    }
    public async Task<IEnumerable<Seed>> GetAll()
    {
        return await dbContext.Seeds.ToListAsync();
    }

    public async Task<Seed?> FindByName(string seedName)
    {
        return await dbContext.Seeds.FirstOrDefaultAsync(x => x.Name == seedName);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await dbContext.Database.BeginTransactionAsync();
    }

    public Task CommitAsync()
    {
        throw new NotImplementedException();
    }

    public Task RollbackAsync()
    {
        throw new NotImplementedException();
    }
}
