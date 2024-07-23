using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders;

public class P_2_ModuleSeeder : ISeeder
{
    private readonly IModuleRepository moduleRepository;
    private readonly IUserRepository userRepository;

    public P_2_ModuleSeeder(IModuleRepository moduleRepository, IUserRepository userRepository)
    {
        this.moduleRepository = moduleRepository;
        this.userRepository = userRepository;
    }
    public async Task up()
    {
        var systemUser = await userRepository.GetSystemUser();
        var modules = new List<Module>{
            new Module{
                Name= Modules.user.ToString(),
                Description= "Module for user management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.role.ToString(),
                Description= "Module for role management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.permission.ToString(),
                Description= "Module for permission management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.module.ToString(),
                Description= "Module for module management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
        };

        moduleRepository.AddRange(modules);

        await moduleRepository.SaveChangesAsync();
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