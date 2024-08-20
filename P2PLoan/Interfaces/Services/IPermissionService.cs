using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;
public interface IPermissionService
{

    Task<ServiceResponse<object>> CreatePermissionAsync(CreatePermissionRequestDto createPermissionRequestDto);


    Task<Permission?> GetPermissionByIdAsync(Guid permissionId);

    Task<ServiceResponse<object>> GetAllPermissionsAsync();


    Task<bool> UpdatePermissionAsync(Permission permission);


    Task<ServiceResponse<object>> DeletePermissionAsync(Guid permissionId);


    Task<bool> CheckPermissionAsync(string userId, string permissionName);
}
