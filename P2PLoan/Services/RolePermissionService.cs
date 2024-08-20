using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Repositories;

namespace P2PLoan.Services
{
    public class RolePermissionService : IRolePermissionService
    {
        private readonly IRolePermissionRepository rolePermissionRepository;
        private readonly IUserRepository userRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IPermissionRepository permissionRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IMapper mapper;
        public RolePermissionService(IRolePermissionRepository rolePermissionRepository,IPermissionRepository permissionRepository, IUserRepository userRepository,IRoleRepository roleRepository,IRoleRepository userRoleRepository,IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            this.permissionRepository = permissionRepository;
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
            this.httpContextAccessor = httpContextAccessor;
            this.rolePermissionRepository = rolePermissionRepository;
            this.mapper = mapper;

        }

        public async Task<ServiceResponse<object>> AttachPermissionToRole(Guid roleId, Guid permissionId,RolePermissionRequestDto rolePermissionRequestDto)
        {
            var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();
            var user = await userRepository.GetByIdAsync(userId);
            if(user == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null); 
            }

            var rolePermission = await rolePermissionRepository.FindByRoleIdandPermissionId(roleId, permissionId);
            if(rolePermission != null)
            {
                  return new ServiceResponse<object>(ResponseStatus.BadRequest,AppStatusCodes.ValidationError,  "The permission is already assigned to this role.",
            rolePermission);
            }
             var rolePermissionDto = new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                PermissionId = permissionId,
                CreatedById = user.Id,
                ModifiedById = user.Id
             };

            mapper.Map(rolePermissionRequestDto, rolePermissionDto);
            rolePermissionRepository.Add(rolePermissionDto);
            await rolePermissionRepository.SaveChangesAsync();

            return new ServiceResponse<object>(
                ResponseStatus.Success, 
                AppStatusCodes.Success, 
                "Permission attached to role successfully.", 
                null
            );   
        }

        public async Task<ServiceResponse<object>> DetachPermissionFromRole(Guid roleId, Guid permissionId)
        {
            var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();
            var user = await userRepository.GetByIdAsync(userId);
            if(user == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null); 
            }
             var rolePermission = await rolePermissionRepository.FindByRoleIdandPermissionId(roleId, permissionId);
             if(rolePermission is null)
             {
                
                  return new ServiceResponse<object>(ResponseStatus.BadRequest,AppStatusCodes.ValidationError,  "No permission is assigned to this role.",
            null);
             }
             rolePermissionRepository.Delete(rolePermission);
             await rolePermissionRepository.SaveChangesAsync();
         

            return new ServiceResponse<object>(
                ResponseStatus.Success,
                AppStatusCodes.Success,
                "permission detached successfully.", null
            );
           
        }
    }
}


