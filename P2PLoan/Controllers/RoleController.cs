using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Attributes;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Interfaces.Services;
using P2PLoan.Models;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/[controller]")]

public class RoleController : ControllerBase
{
    private readonly IRoleService roleService;
    private readonly IUserRoleService userRoleService;
    public RoleController(IRoleService roleService, IUserRoleService userRoleService)
    {
        this.roleService = roleService;
        this.userRoleService = userRoleService;

    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequestDto createRoleRequestDto)
    {
        var response = await roleService.CreateRole(createRoleRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    [Route("{id:guid}")]
     [Authorize]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var response = await roleService.GetRoleById(id);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
     [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var response = await roleService.GetAllRole();
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPatch]
    [Route("{id:guid}")]
     [Authorize]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleRequestDto updateRoleRequestDto)
    {
        var response = await roleService.UpdateRoleById(id, updateRoleRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost("attach")]
     [Authorize]
     public async Task<IActionResult> AttachRoleToUser([FromBody] UserRoleRequestDto userRoleDto )
    {
            var response = await userRoleService.AttachRoleToUser(userRoleDto.UserId, userRoleDto.RoleId, userRoleDto);
            return ControllerHelper.HandleApiResponse(response);
    }
      
    [HttpPost("detach")]
     [Authorize]
    public async Task<IActionResult> DetachRoleFromUser([FromBody] UserRoleRequestDto userRoleDto)
    {
            var response = await userRoleService.DetachRoleFromUser(userRoleDto.UserId, userRoleDto.RoleId);
            return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("all")]
     [Authorize]
    public async Task<IActionResult> GetAllUserRole()
    {
            var response = await userRoleService.GetAllUserRolesAsync();
            return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("role/{id:guid}")]
     [Authorize]
    public async Task<IActionResult> GetUserRoleByRoleId(Guid id)
    {
            var response = await userRoleService.GetUserRolesByRoleId(id);
            return ControllerHelper.HandleApiResponse(response);

    }
    [HttpGet("user/{id:guid}")]
     [Authorize]
    public async Task<IActionResult> GetUserRoleByUserId(Guid id)
    {
        var response = await userRoleService.GetUserRolesByUserId(id);
        return ControllerHelper.HandleApiResponse(response);
    } 

    [HttpGet("userrole/{id:guid}")]
     [Authorize]
    public async Task<IActionResult> GetUserRoleById(Guid id)
    {
        var response = await userRoleService.GetUserRoleById(id);
        return ControllerHelper.HandleApiResponse(response);
    }

      

    [HttpDelete]
     [Authorize]
    [Route("{id:guid}")]

    public async Task<IActionResult> Delete(Guid id)
    {
        var response = await roleService.DeleteRole(id);
        return ControllerHelper.HandleApiResponse(response);
    }

}