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

       [HttpPost("attach")]
        public async Task<IActionResult> AttachRoleToUser([FromBody] UserRoleDto userRoleDto )
        {
            var response = await userRoleService.AttachRoleToUser(userRoleDto.UserId, userRoleDto.RoleId);
            return ControllerHelper.HandleApiResponse(response);
        }

        [HttpPost("detach")]
        public async Task<IActionResult> DetachRoleFromUser([FromBody] UserRoleDto userRoleDto)
        {
            var response = await userRoleService.DetachRoleFromUser(userRoleDto.UserId, userRoleDto.RoleId);
            return ControllerHelper.HandleApiResponse(response);
        }

       [HttpGet("all")]
         public async Task<IActionResult> GetAllUserRole()
        {
            var response = await userRoleService.GetAllUserRolesAsync();
            return ControllerHelper.HandleApiResponse(response);
        }

        [HttpGet("role/{id:guid}")]
        public async Task<IActionResult> GetUserRoleByRoleId(Guid id)
        {
            var response = await userRoleService.GetUserRolesByRoleId(id);
            return ControllerHelper.HandleApiResponse(response);

        }
        [HttpGet("user/{id:guid}")]
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