using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan;

public class PermissionRepository : IPermissionRepository
{
    private readonly P2PLoanDbContext dbContext;

    public PermissionRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(Permission permission)
    {
        dbContext.Permissions.Add(permission);
    }

    public void AddRange(IEnumerable<Permission> permissions)
    {
        dbContext.Permissions.AddRange(permissions);
    }

    public async Task<Permission?> FindByModuleAndAction(Guid moduleId, PermissionAction action)
    {
        return await dbContext.Permissions.FirstOrDefaultAsync(x => x.ModuleId == moduleId && x.Action == action);
    }

    public async Task<IEnumerable<Permission>> GetAll()
    {
        return await dbContext.Permissions.ToListAsync();
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}
