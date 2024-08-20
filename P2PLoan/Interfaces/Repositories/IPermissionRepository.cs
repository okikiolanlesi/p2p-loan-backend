using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces
{
    public interface IPermissionRepository
    {
        void Add(Permission permission);

        void AddRange(IEnumerable<Permission> permissions);

        Task<Permission?> FindByModuleAndAction(Guid moduleId, PermissionAction action);

        Task<IEnumerable<Permission>> GetAll();
        Task<Permission?> GetByIdAsync(Guid id);

        void Delete(Permission permission);


        Task<bool> SaveChangesAsync();


    }
}

