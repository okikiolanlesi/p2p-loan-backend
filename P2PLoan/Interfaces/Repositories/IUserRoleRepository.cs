using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IUserRoleRepository
{
    void Add(UserRole userRole);
    Task<UserRoleDto?> FindById(Guid userRoleId);
    Task<IEnumerable<UserRoleDto?>> FindAllByUserId(Guid userId);
    Task<IEnumerable<UserRoleDto?>> FindAllByRoleId(Guid roleId);
    Task<IEnumerable<UserRoleDto?>> GetAll();
    Task<UserRole> FindByUserIdAndRoleId(Guid userId, Guid roleId);
    void AddRange(IEnumerable<UserRole> userRoles);
    void MarkAsModified(UserRole userRole);
    Task<bool> SaveChangesAsync();
    void Delete (UserRole userRole);
}
