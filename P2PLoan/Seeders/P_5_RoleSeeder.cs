using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders;

public class P_5_RoleSeeder : ISeeder
{
    private readonly IRoleRepository roleRepository;
    private readonly IUserRepository userRepository;
    private readonly IPermissionRepository permissionRepository;
    private readonly IRolePermissionRepository rolePermissionRepository;

    public P_5_RoleSeeder(IRoleRepository roleRepository, IUserRepository userRepository, IPermissionRepository permissionRepository, IRolePermissionRepository rolePermissionRepository)
    {
        this.roleRepository = roleRepository;
        this.userRepository = userRepository;
        this.permissionRepository = permissionRepository;
        this.rolePermissionRepository = rolePermissionRepository;
    }
    public async Task up()
    {
        var systemUser = await userRepository.GetSystemUser();

        var adminRole = new Role
        {
            Id = Guid.NewGuid(),
            Name = "Super Admin",
            Description = "Role for super admins",
            CreatedBy = systemUser,
            ModifiedBy = systemUser,
        };

        roleRepository.Add(adminRole);

        await roleRepository.SaveChangesAsync();

        var permissions = await permissionRepository.GetAll();

        var permissionsForSuperAdmin = new List<RolePermission>();

        foreach (var permission in permissions)
        {
            permissionsForSuperAdmin.Add(new RolePermission
            {
                Id = Guid.NewGuid(),
                Role = adminRole,
                Permission = permission,
                CreatedBy = systemUser,
                ModifiedBy = systemUser,

            });
        }

        rolePermissionRepository.AddRange(permissionsForSuperAdmin);

        await rolePermissionRepository.SaveChangesAsync();

    }
    public async Task down()
    {
        throw new NotImplementedException();
    }
    public string Description()
    {
        return "";
    }
}
