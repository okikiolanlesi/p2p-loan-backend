using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using P2PLoan.Constants;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class ModuleRepository : IModuleRepository
{
    private readonly P2PLoanDbContext dbContext;

    public ModuleRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(Module module)
    {
        dbContext.Modules.Add(module);
    }

    public void AddRange(IEnumerable<Module> modules)
    {
        dbContext.Modules.AddRange(modules);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await dbContext.Database.BeginTransactionAsync();
    }

    public async Task<IEnumerable<Module>> GetAllAsync()
    {
        return await dbContext.Modules.ToListAsync();
    }

    public async Task<Module> GetModuleByIdentifierAsync(Modules identifier)
    {
        return await dbContext.Modules
                         .FirstOrDefaultAsync(m => m.Identifier == identifier);
    }

    public async Task<Module> GetModuleByIdAsync(Guid id)
    {
        return await dbContext.Modules
                         .FirstOrDefaultAsync(m=>m.Id == id);
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
