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
[Route("api/role")]

public class RoleController : ControllerBase
{
    private readonly IRoleService roleService;
    public RoleController(IRoleService roleService)
    {
        this.roleService = roleService;

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto createRoleRequestDto)
    {
        var response = await roleService.CreateRole(createRoleRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var response = await roleService.GetRoleById(id);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await roleService.GetAllRole();
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPatch]
    [Route("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequestDto updateRoleRequestDto)
    {
        var response = await roleService.UpdateRoleById(id, updateRoleRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpDelete]
    [Route("{id:guid}")]

    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await roleService.DeleteRole(id);
        return ControllerHelper.HandleApiResponse(response);
    }

}