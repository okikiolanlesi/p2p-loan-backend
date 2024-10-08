﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly P2PLoanDbContext dbContext;

    public RoleRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public void Add(Role role)
    {
        dbContext.Roles.Add(role);
    }

    public void AddRange(IEnumerable<Role> roles)
    {
        dbContext.Roles.AddRange(roles);
    }

    public async Task<Role> FindById(Guid roleId)
    {
        return await dbContext.Roles.
        Include(r=>r.Permissions).
        FirstOrDefaultAsync(x => x.Id == roleId);
    }

    public async Task<IEnumerable<Role>> GetAll()
    {
        return await dbContext.Roles.ToListAsync();
    }

    public void MarkAsModified(Role role)
    {
        dbContext.Entry(role).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> RoleExistsAsync(string roleName)
    {
        return await dbContext.Roles.AnyAsync(r => r.Name == roleName);
    }

    public void Remove(Role role)
    {
       dbContext.Roles.Remove(role);
    }
}
