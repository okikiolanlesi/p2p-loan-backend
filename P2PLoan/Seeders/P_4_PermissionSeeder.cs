using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders;

public class P_4_PermissionSeeder : ISeeder
{
    private readonly IPermissionRepository permissionRepository;
    private readonly IUserRepository userRepository;
    private readonly IModuleRepository moduleRepository;

    public P_4_PermissionSeeder(IPermissionRepository permissionRepository, IUserRepository userRepository, IModuleRepository moduleRepository)
    {
        this.permissionRepository = permissionRepository;
        this.userRepository = userRepository;
        this.moduleRepository = moduleRepository;
    }
    public async Task up()
    {
        var systemUser = await userRepository.GetSystemUser();

        var permissionActions = new List<PermissionAction>(){
            PermissionAction.create, PermissionAction.read, PermissionAction.update, PermissionAction.delete
        };
        var modules = await moduleRepository.GetAllAsync();

        var permissions = new List<Permission>();

        foreach (var module in modules)
        {

            foreach (var permissionAction in permissionActions)
            {
                permissions.Add(new Permission
                {
                    Id = Guid.NewGuid(),
                    ModuleId = module.Id,
                    Action = permissionAction,
                    CreatedBy = systemUser,
                    ModifiedBy = systemUser
                });
            }
        }

        permissionRepository.AddRange(permissions);

        await permissionRepository.SaveChangesAsync();
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
