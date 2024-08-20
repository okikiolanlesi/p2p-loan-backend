using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolePermissionController : ControllerBase
    {
        private readonly IRolePermissionRepository RolePermissionRepository;

        public RolePermissionController(IRolePermissionRepository rolePermissionRepository)
        {
            this.RolePermissionRepository = rolePermissionRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RolePermission>>> GetAllRolePermissions()
        {
            var rolePermissions = await this.RolePermissionRepository.GetAll();
            return Ok(rolePermissions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RolePermission>> GetRolePermissionById(Guid id)
        {
            var rolePermission = await this.RolePermissionRepository.FindById(id);
            if (rolePermission == null)
            {
                return NotFound();
            }
            return Ok(rolePermission);
        }

        [HttpPost]
        public async Task<ActionResult> AddRolePermission(RolePermission rolePermission)
        {
            this.RolePermissionRepository.Add(rolePermission);
            await this.RolePermissionRepository.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRolePermissionById), new { id = rolePermission.Id }, rolePermission);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRolePermission(Guid id, RolePermission rolePermission)
        {
            if (id != rolePermission.Id)
            {
                return BadRequest();
            }

            this.RolePermissionRepository.MarkAsModified(rolePermission);
            await this.RolePermissionRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteRolePermission(Guid id)
        {
            var rolePermission = await this.RolePermissionRepository.FindById(id);
            if (rolePermission == null)
            {
                return NotFound();
            }

            this.RolePermissionRepository.MarkAsModified(rolePermission);
            await this.RolePermissionRepository.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("permission/{permissionId}")]
        public async Task<ActionResult<IEnumerable<RolePermission>>> GetRolePermissionsByPermissionId(Guid permissionId)
        {
            var rolePermissions = await this.RolePermissionRepository.FindAllByPermissionId(permissionId);
            return Ok(rolePermissions);
        }

        [HttpGet("role/{roleId}")]
        public async Task<ActionResult<IEnumerable<RolePermission>>> GetRolePermissionsByRoleId(Guid roleId)
        {
            var rolePermissions = await this.RolePermissionRepository.FindAllByRoleId(roleId);
            return Ok(rolePermissions);
        }
    }
}
