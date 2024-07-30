using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan;

public interface IRoleRepository
{
    void Add(Role role);
    Task<Role?> FindById(Guid roleId);
    Task<IEnumerable<Role>> GetAll();
    void AddRange(IEnumerable<Role> roles);
    void MarkAsModified(Role role);
    Task<bool> SaveChangesAsync();
}
