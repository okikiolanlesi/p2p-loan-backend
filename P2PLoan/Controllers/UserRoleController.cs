using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces.Services;

namespace P2PLoan.Controllers
{
   [ApiController]
   [Route("api/[controller]")]
    public class UserRoleController : Controller
    {
        private readonly IUserRoleService userRoleService;
        private readonly IMapper mapper;

        public UserRoleController(IUserRoleService userRoleService, IMapper mapper)
        {
           this.userRoleService = userRoleService;
           this.mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> AttachRoleToUser([FromBody] UserRoleDto userRoleDto )
        {
            // Extract UserId and RoleId from the DTO
            var userId = userRoleDto.UserId;
            var roleId = userRoleDto.RoleId;
            var response = await userRoleService.AttachRoleToUser(userId, roleId);
            return ControllerHelper.HandleApiResponse(response);
        }

        [HttpGet]
         public async Task<IActionResult> GetAllUserRole()
        {
            var response = await userRoleService.GetAllUserRolesAsync();
            return ControllerHelper.HandleApiResponse(response);
        }

        [HttpGet("roles/{id:guid}")]
        public async Task<IActionResult> GetUserRoleByRoleId(Guid id)
        {
            var response = await userRoleService.GetUserRolesByRoleId(id);
            return ControllerHelper.HandleApiResponse(response);

        }
        [HttpGet("users/{id:guid}")]
       public async Task<IActionResult> GetUserRoleByUserId(Guid id)
       {
        var response = await userRoleService.GetUserRolesByUserId(id);
        return ControllerHelper.HandleApiResponse(response);
       } 

       [HttpGet("{id:guid}")]
       public async Task<IActionResult> GetUserRoleById(Guid id)
       {
        var response = await userRoleService.GetUserRoleById(id);
        return ControllerHelper.HandleApiResponse(response);
       }

      
    }
}