using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IModuleService
{
  
    Task<ServiceResponse<object>> GetModuleByIdAsync(Guid id);
    Task<ServiceResponse<object>> UpdateModuleByIdAsync(Guid id, UpdateModuleRequestDto updateModuleRequestDto);
    Task<ServiceResponse<IEnumerable<Module>>> GetAllModule();

}