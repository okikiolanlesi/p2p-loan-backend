using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository roleRepository;
        private IMapper mapper;
        public RoleService(IRoleRepository roleRepository,IMapper mapper)
        {
            this.roleRepository = roleRepository;
            this.mapper = mapper;
        

        }
        public async Task<ServiceResponse<object>> CreateRole(CreateRoleRequestDto createRoleRequestDto)
        {
            // Check if a role with the same name already exists
            if (await roleRepository.RoleExistsAsync(createRoleRequestDto.Name))
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Role with the same name already exists.", null);
            }
            var role = mapper.Map<Role>(createRoleRequestDto);
            roleRepository.Add(role);
            await roleRepository.SaveChangesAsync();

             // Logic for adding permission to the role


            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Role created successfully.", role);
        }

        public async Task<ServiceResponse<object>> DeleteRole(Guid id)
        {
           // Find the role by its ID
        var roleToDelete = await roleRepository.FindById(id);

        if (roleToDelete == null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Role not found", null);
        }

        // Remove the role
        roleRepository.Remove(roleToDelete);
        await roleRepository.SaveChangesAsync();

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Role deleted successfully", null);
        }

        public async Task<ServiceResponse<object>> GetAllRole()
        {
            var roles = await roleRepository.GetAll();
            if(roles is null)
            {

            }

            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success,"Roles retrieved",roles);

        }

        public async Task<ServiceResponse<object>> GetRoleById(Guid id)
        {
            var role = await roleRepository.FindById(id);
            if(role is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Role does not exist.", null);
            }
            // Return a successful response with the role data
            return new ServiceResponse<object>(  ResponseStatus.Success,  AppStatusCodes.Success,   "Role retrieved successfully.",  role
            );
        }

        public async Task<ServiceResponse<object>> UpdateRoleById(Guid id, UpdateRoleRequestDto updateRoleRequestDto)
        {
            // check for role
            var existingRole = await roleRepository.FindById(id);
            if(existingRole is null)
            {
             return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Role does not exist.", null);
            }

            mapper.Map(updateRoleRequestDto, existingRole);
            await roleRepository.SaveChangesAsync();

            // Logic for adding permission to the role

            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Role updated successfully.", existingRole);

        }
    }
}