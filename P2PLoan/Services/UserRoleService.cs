using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using P2PLoan.Constants;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Interfaces.Services;
using P2PLoan.Models;

namespace P2PLoan.Services
{
    public class UserRoleService : IUserRoleService
    {
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUserRoleRepository userRoleRepository;
        public UserRoleService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;

        }
        public async Task<ServiceResponse<object>> GetAllUserRolesAsync()
        {
            var userRole = await userRoleRepository.GetAll();
            if(userRole is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "id does not exist.", null);
            }
             return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "userRole retrieved succesfully.", userRole);
            
        }

        public async Task<ServiceResponse<object>> GetUserRoleById(Guid id)
        {
            var userRole = await userRoleRepository.FindById(id);
            if(userRole == null)
               {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "id does not exist.", null);
            }

             return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "userRole retrieved succesfully.", userRole);
            
        }

        public async Task<ServiceResponse<object>> GetUserRolesByRoleId(Guid roleId)
        {
            var userRole = await userRoleRepository.FindAllByRoleId(roleId);
            if(userRole is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "id does not exist.", null);

            }
             return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "userRole retrieved succesfully.", userRole); 
        }

        public async Task<ServiceResponse<object>> GetUserRolesByUserId(Guid userId)
        {
            var userRole = await userRoleRepository.FindAllByUserId(userId);
            if(userRole is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "id does not exist.", null);
            }
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "userRole retrieved succesfully.", userRole); 
            
        }

        public async Task<ServiceResponse<object>> AttachRoleToUser(Guid id, Guid roleId)
        {
           // Check if the UserRole association exists using the optimized method
            var userRole = await userRoleRepository.FindByUserIdAndRoleId(id, roleId);
            if (userRole == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest,AppStatusCodes.ValidationError, "The user is not assigned to this role or user/role does not exist.",null);
            }
            // Create and add the new UserRole
            var user_role = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = id,
                RoleId = roleId
            };
            userRoleRepository.Add(user_role);
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "userRole retrieved succesfully.", user_role); 

        }

         public async Task<ServiceResponse<object>> DetachRoleFromUser(Guid id, Guid roleId)
         {
            // Check if the UserRole association exists using the optimized method
            var userRole = await userRoleRepository.FindByUserIdAndRoleId(id, roleId);
            if (userRole == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest,AppStatusCodes.ValidationError,"The user is not assigned to this role or user/role does not exist.",null);
            }
            // Remove the UserRole
            userRoleRepository.Delete(userRole);
            return new ServiceResponse<object>( ResponseStatus.Success, AppStatusCodes.Success,"Role detached successfully.",userRole);
        }
    }
}