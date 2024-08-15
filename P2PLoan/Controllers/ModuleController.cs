using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Attributes;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/module")]

public class ModuleController : ControllerBase
{
    private readonly IModuleService moduleService;
    public  ModuleController(IModuleService moduleService)
    {
        this.moduleService = moduleService;      
    }

    [HttpPost]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleRequestDto createModuleRequestDto)
    {
        var response = await moduleService.CreateModuleAsync(createModuleRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetModuleById(Guid id)
    {
        var response = await moduleService.GetModuleByIdAsync(id);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]   
    public async Task<IActionResult> GetAll()
    {
        var response = await  moduleService.GetAllModule();
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPatch]
     [Route("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateModuleRequestDto updateModuleRequestDto)
    {
        var response = await moduleService.UpdateModuleByIdAsync(id, updateModuleRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }






}