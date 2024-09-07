using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IRoleService
{
    Task<ServiceResponse<object>> CreateRole(CreateRoleRequestDto createRoleRequestDto);
    Task<ServiceResponse<object>> GetAllRole();
    Task<ServiceResponse<object>> GetRoleById(Guid id);
    Task<ServiceResponse<object>> UpdateRoleById(Guid id, UpdateRoleRequestDto  updateRoleRequestDto);
    Task<ServiceResponse<object>> DeleteRole(Guid id);

}