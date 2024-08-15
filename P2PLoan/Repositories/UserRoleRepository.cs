using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly P2PLoanDbContext dbContext;
    private readonly IMapper mapper;

    public UserRoleRepository(P2PLoanDbContext dbContext, IMapper mapper)
    {
        this.dbContext = dbContext;
        this.mapper = mapper;
    }
    public void Add(UserRole userRole)
    {
        dbContext.UserRoles.Add(userRole);
    }

    public void AddRange(IEnumerable<UserRole> userRoles)
    {
        dbContext.UserRoles.AddRange(userRoles);
    }

    public async Task<UserRoleDto?> FindById(Guid id)
    {
        return await dbContext.UserRoles
                               .Include(x=>x.Role)
                               .Include(x=>x.User).ProjectTo<UserRoleDto>(mapper.ConfigurationProvider)
                               .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<UserRoleDto?>> FindAllByRoleId(Guid roleId)
    {
        return await dbContext.UserRoles
                              .Include(x=>x.Role)
                              .Include(x=>x.User).ProjectTo<UserRoleDto>(mapper.ConfigurationProvider)
                              .Where(x => x.RoleId == roleId)
                              .ToListAsync();
    }

    public async Task<IEnumerable<UserRoleDto>> FindAllByUserId(Guid userId)
    {
        return await dbContext.UserRoles
                              .Include(x=>x.Role)
                              .Include(x=>x.User).ProjectTo<UserRoleDto>(mapper.ConfigurationProvider)
                              .Where(x => x.UserId == userId)
                              .ToListAsync();
    }

    public async Task<IEnumerable<UserRoleDto>> GetAll()
    {
        return await dbContext.UserRoles
                               .Include(u=>u.Role)
                               .Include(u=>u.User).ProjectTo<UserRoleDto>(mapper.ConfigurationProvider)
                               .ToListAsync();
    }

    public void MarkAsModified(UserRole userRole)
    {
        dbContext.Entry(userRole).State = EntityState.Modified;
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }

    public void Delete(UserRole userRole)
    {
        dbContext.Remove(userRole);
    }

    public async Task<UserRole> FindByUserIdAndRoleId(Guid userId, Guid roleId)
    {
        return await dbContext.UserRoles
          .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);

    }
}
