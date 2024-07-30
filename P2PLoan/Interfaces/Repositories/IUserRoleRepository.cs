using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IUserRoleRepository
{
    void Add(UserRole userRole);
    Task<UserRole?> FindById(Guid userRoleId);
    Task<IEnumerable<UserRole>> FindAllByUserId(Guid userId);
    Task<IEnumerable<UserRole>> FindAllByRoleId(Guid roleId);
    Task<IEnumerable<UserRole>> GetAll();
    void AddRange(IEnumerable<UserRole> userRoles);
    void MarkAsModified(UserRole userRole);
    Task<bool> SaveChangesAsync();
}
