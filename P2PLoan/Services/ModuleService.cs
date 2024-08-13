using System;
using System.Collections.Generic;
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
    public class ModuleService : IModuleService
    {
        private readonly IModuleRepository moduleRepository;
        private readonly IMapper mapper;
        public ModuleService(IModuleRepository moduleRepository, IMapper mapper)
        {
            this.moduleRepository = moduleRepository;
            this.mapper = mapper;
        }
        public async Task<ServiceResponse<object>> CreateModuleAsync(CreateModuleRequestDto createModuleRequestDto)
        {
            using var transaction = await moduleRepository.BeginTransactionAsync();
            try
            {
                var existingModule = await moduleRepository.GetModuleByIdentifierAsync(createModuleRequestDto.Identifier);

                if (existingModule != null)
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "A module with the same identifier already exists.", null);
                }
                // Map the CreateModuleRequestDto to a Module entity
                var module = mapper.Map<Module>(createModuleRequestDto);
                moduleRepository.Add(module);
                await moduleRepository.SaveChangesAsync();

                // Commit the transaction if all operations succeed
                await transaction.CommitAsync();
                return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes. Success, "Module created successfully.", module);
            }
              catch (Exception)
            {
                // Rollback the transaction if an error occurs
                await transaction.RollbackAsync();
                throw;
            }
 
        }

        public async Task<ServiceResponse<IEnumerable<Module>>> GetAllModule()
        {
            var modules = await moduleRepository.GetAllAsync();
            // Return a successful response with the list of modules
           return new ServiceResponse<IEnumerable<Module>>(ResponseStatus.Success, AppStatusCodes.Success, "Modules retrieved successfully", modules);
        }

        public async Task<ServiceResponse<object>> GetModuleByIdAsync(Guid id)
        {
            var module = await moduleRepository.GetModuleByIdAsync(id);
            if (module is null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Module not found", null);
            }
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Module retrieved successfully", module);      
        }

        public async Task<ServiceResponse<object>> UpdateModuleByIdAsync(Guid id, UpdateModuleRequestDto updateModuleRequestDto)
        {
            using var transaction = await moduleRepository.BeginTransactionAsync();
        try
        {
            // Retrieve the existing module by ID
            var existingModule = await moduleRepository.GetModuleByIdAsync(id);

            if (existingModule == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Module not found", null);
            }

            // Check if another module with the same Identifier already exists
            var existingIdentifierModule = await moduleRepository.GetModuleByIdentifierAsync(updateModuleRequestDto.Identifier);
            
            if (existingIdentifierModule != null && existingIdentifierModule.Id != id)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "A module with the same identifier already exists.", null);
            }

        // Use AutoMapper to map the properties from updateModuleRequestDto to existingModule
                mapper.Map(updateModuleRequestDto, existingModule);
            
            await moduleRepository.SaveChangesAsync();

            await transaction.CommitAsync();

                return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Module updated successfully", existingModule);
            }
            catch (Exception ex)
            {
                // Rollback transaction in case of error
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
    

