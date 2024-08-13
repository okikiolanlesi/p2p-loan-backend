using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IRoleRepository
{    void Add(Role role);
     void Remove(Role role);
    Task<Role?> FindById(Guid roleId);
    Task<IEnumerable<Role>> GetAll();
    void AddRange(IEnumerable<Role> roles);
    void MarkAsModified(Role role);
    Task<bool> SaveChangesAsync();
    Task<bool> RoleExistsAsync(string roleName);
}
