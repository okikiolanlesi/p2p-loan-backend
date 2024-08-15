using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using P2PLoan.Constants;
using P2PLoan.DTOs;
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
        private readonly IMapper mapper;
        public UserRoleService(IUserRepository userRepository, IRoleRepository roleRepository, IUserRoleRepository userRoleRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.userRoleRepository = userRoleRepository;
            this.mapper = mapper;

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
            if(userRole == null || !userRole.Any())
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "id does not exist.", null);

            }
             return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "userRole retrieved succesfully.", userRole); 
        }

        public async Task<ServiceResponse<object>> GetUserRolesByUserId(Guid userId)
        {
            var userRole = await userRoleRepository.FindAllByUserId(userId);
            if(userRole == null || !userRole.Any())
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "id does not exist.", null);
            }
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "userRole retrieved succesfully.", userRole); 
            
        }

        public async Task<ServiceResponse<object>> AttachRoleToUser(Guid id, Guid roleId,UserRoleDto userRoleDto)
        {
           // Check if the UserRole association exists using the optimized method
            var userRole = await userRoleRepository.FindByUserIdAndRoleId(id, roleId);
            if (userRole != null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest,AppStatusCodes.ValidationError,  "The user is already assigned to this role.",
            null);
            } 
            var newUserRole = new UserRole
            {
                UserId = id,  
                RoleId = roleId,
            };

            mapper.Map(userRoleDto, newUserRole);
            userRoleRepository.Add(newUserRole);
            var success = await userRoleRepository.SaveChangesAsync();
            if (!success)
            {
                return new ServiceResponse<object>(
                    ResponseStatus.BadRequest,
                    AppStatusCodes.ValidationError,
                    "An error occurred while attaching the role to the user.",
                    null
                );
            }

            return new ServiceResponse<object>(
                ResponseStatus.Success,
                AppStatusCodes.Success,
                "Role attached to user successfully.",
                newUserRole
            );

        }

         public async Task<ServiceResponse<object>> DetachRoleFromUser(Guid id, Guid roleId)
         {
            // Check if the UserRole association exists using the optimized method
            var userRole = await userRoleRepository.FindByUserIdAndRoleId(id, roleId);
            if (userRole == null)
            {
                // The role is not assigned to the user
                return new ServiceResponse<object>(
                    ResponseStatus.BadRequest,
                    AppStatusCodes.ValidationError,
                    "The user is not assigned to this role or user/role does not exist.",
                    null
                );
            }

            // Remove the UserRole
            userRoleRepository.Delete(userRole);

            // Save changes to the database
            var success = await userRoleRepository.SaveChangesAsync();
            if (!success)
            {
                return new ServiceResponse<object>(
                    ResponseStatus.BadRequest,
                    AppStatusCodes.ResourceNotFound,
                    "An error occurred while detaching the role from the user.",
                    null
                );
            }

            return new ServiceResponse<object>
            (
                ResponseStatus.Success,
                AppStatusCodes.Success,
                "Role detached successfully.",
                userRole
            );
        }
    }
}