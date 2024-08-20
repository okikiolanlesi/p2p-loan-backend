using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Helpers;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            this.permissionService = permissionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreatePermission([FromBody] CreatePermissionRequestDto createPermissionRequestDto)
        {
            var response = await permissionService.CreatePermissionAsync(createPermissionRequestDto);
            return ControllerHelper.HandleApiResponse(response);

        }

        [HttpDelete]
        public async Task<IActionResult> DeletePermission(Guid id)
        {
            var response = await permissionService.DeletePermissionAsync(id);
            return ControllerHelper.HandleApiResponse(response);
        }


    }
}
