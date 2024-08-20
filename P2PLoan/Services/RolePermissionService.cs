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



        public RolePermissionService()
        {

        }

        public async Task<ServiceResponse<object>> AttachPermissionToRole(Guid permissionId, CreatePermissionRequestDto createPermissionDto)
        {
            var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();
            var user = await permissionRepository.GetByIdAsync(permissionId);
            if (user is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
            }
            var rolePermission = await permissionRepository.FindByIdAsync(permissionId);
            if (rolePermission != null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
            }
            var newPermissionRole = new PermissionRole
            {
                PermissionId = id,
                RoleId = roleId,
                CreatedById = permission.Id,
                ModifiedById = permission.Id
            };

            mapper.Map(createPermissionDto, newPermissionRole);
            permissionRepository.Add(newPermissionRole);
            var success = await PermissionRepository.SaveChangesAsync();
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
                "Role attached to user successfully.", null
            );

        }
    }

    public Task<ServiceResponse<object>> DetachPermissionFromRole()
    {
        var newUserRole = new UserRole
        {
            UserId = id,
            RoleId = roleId,
            CreatedById = user.Id,
            ModifiedById = user.Id
        };

        mapper.Map(CreatePermissionRequestDto, newPermissionRole);
        PermissionRepository.Add(newpermissionRole);
        var success = await permissionRepository.SaveChangesAsync();
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
            "Role attached to user successfully.", null
        ); ;
    }
}

}
