using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class RolePermissionRepository : IRolePermissionRepository
{
    private readonly P2PLoanDbContext dbContext;

    public RolePermissionRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(RolePermission rolePermission)
    {
        dbContext.RolePermissions.Add(rolePermission);
    }

    public void AddRange(IEnumerable<RolePermission> rolePermissions)
    {
        dbContext.RolePermissions.AddRange(rolePermissions);
    }

    public async Task<IEnumerable<RolePermission>> FindAllByPermissionId(Guid permissionId)
    {
        return await dbContext.RolePermissions.Where(x => x.PermissionId == permissionId).ToListAsync();
    }

    public async Task<IEnumerable<RolePermission>> FindAllByRoleId(Guid roleId)
    {
        return await dbContext.RolePermissions.Where(x => x.RoleId == roleId).ToListAsync();
    }

    public async Task<RolePermission> FindById(Guid rolePermissionId)
    {
        return await dbContext.RolePermissions.FirstOrDefaultAsync(x => x.Id == rolePermissionId);
    }

    public async Task<RolePermission> FindByRoleIdandPermissionId(Guid roleId, Guid permissionId)
    {
        return await dbContext.RolePermissions.FirstOrDefaultAsync(x=>x.RoleId==roleId && x.PermissionId==permissionId);
    }

    public async Task<IEnumerable<RolePermission>> GetAll()
    {
        return await dbContext.RolePermissions.ToListAsync();
    }

    public void MarkAsModified(RolePermission rolePermission)
    {
        dbContext.Entry(rolePermission).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }

     public void Delete(RolePermission rolePermission)
    {
        dbContext.Remove(rolePermission);
    }

}
