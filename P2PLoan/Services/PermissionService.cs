using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Repositories;

namespace P2PLoan.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly IPermissionRepository permissionRepository;
        private readonly IMapper mapper;


        public PermissionService(IPermissionRepository permissionRepository, IMapper mapper)
        {
            this.permissionRepository = permissionRepository;
            //this.httpContextAccessor = httpContextAccessor;
            // this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public Task<bool> CheckPermissionAsync(string userId, string permissionName)
        {
            throw new NotImplementedException();
        }

        public async Task<ServiceResponse<object>> CreatePermissionAsync(CreatePermissionRequestDto createPermissionRequestDto)
        {


            var permission = mapper.Map<Permission>(createPermissionRequestDto);

            permissionRepository.Add(permission);
            await permissionRepository.SaveChangesAsync();
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "permission created successfully.", null);
        }

        public async Task<ServiceResponse<object>> DeletePermissionAsync(Guid permissionId)
        {
            var permission = await permissionRepository.GetByIdAsync(permissionId);
            if (permission == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Permission does not exist.", null);
            }
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Permission delected successfully.", null);
        }

        public async Task<ServiceResponse<object>> GetAllPermissionsAsync()
        {
            var permission = await permissionRepository.GetAll();
            if (permission == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Permission does not exist.", null);

            }
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Permission created successfully.", permission);

        }

        public Task<Permission> GetPermissionByIdAsync(Guid permissionId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdatePermissionAsync(Permission permission)
        {
            throw new NotImplementedException();
        }
    }

}
