using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
