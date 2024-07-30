using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IPermissionRepository
{
    void Add(Permission permission);
    Task<Permission?> FindByModuleAndAction(Guid moduleId, PermissionAction action);
    Task<IEnumerable<Permission>> GetAll();
    void AddRange(IEnumerable<Permission> permissions);
    Task<bool> SaveChangesAsync();

}
