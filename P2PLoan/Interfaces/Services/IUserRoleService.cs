using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces.Services
{
    public interface IUserRoleService
    {
        Task<ServiceResponse<object>> GetUserRoleById(Guid id);
        Task<ServiceResponse<object>> GetAllUserRolesAsync();
        Task<ServiceResponse<object>> GetUserRolesByUserId(Guid userId);
        Task<ServiceResponse<object>> GetUserRolesByRoleId(Guid roleId);
        Task<ServiceResponse<object>> DetachRoleFromUser(Guid id, Guid roleId);
         Task<ServiceResponse<object>> AttachRoleToUser(Guid id, Guid roleId);
              
    }
}