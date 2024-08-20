using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;
public interface IRolePermissionService
{
    Task<ServiceResponse<object>> AttachPermissionToRole(Guid permissionId);
    Task<ServiceResponse<object>> DetachPermissionFromRole();

}
