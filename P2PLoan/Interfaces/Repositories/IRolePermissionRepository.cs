using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IRolePermissionRepository
{
    void Add(RolePermission rolePermission);
    Task<RolePermission?> FindById(Guid rolePermissionId);
    Task<IEnumerable<RolePermission>> FindAllByPermissionId(Guid permissionId);
    Task<IEnumerable<RolePermission>> FindAllByRoleId(Guid roleId);
    Task<IEnumerable<RolePermission>> GetAll();
    void AddRange(IEnumerable<RolePermission> rolePermissions);
    void MarkAsModified(RolePermission rolePermission);
    Task<bool> SaveChangesAsync();
   
}
