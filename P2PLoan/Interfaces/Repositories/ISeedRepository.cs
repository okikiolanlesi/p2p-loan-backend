using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace P2PLoan.Interfaces;

public interface ISeedRepository
{
    void Add(Seed seed);
    Task<Seed?> FindByName(string seedName);
    Task<IEnumerable<Seed>> GetAll();
    void AddRange(IEnumerable<Seed> seeds);
    Task<bool> SaveChangesAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();

}
