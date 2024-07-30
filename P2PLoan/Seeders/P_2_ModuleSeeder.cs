﻿using System;
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
            new Module{
                Name= Modules.loan.ToString(),
                Description= "Module for loan management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.loanRequest.ToString(),
                Description= "Module for loan request management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.loanOffer.ToString(),
                Description= "Module for loan offer management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.repayment.ToString(),
                Description= "Module for repayment management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.wallet.ToString(),
                Description= "Module for wallet management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.walletProvider.ToString(),
                Description= "Module for wallet provider management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.notification.ToString(),
                Description= "Module for notification management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.notificationTemplate.ToString(),
                Description= "Module for notification template management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.wallet.ToString(),
                Description= "Module for wallet management",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
            new Module{
                Name= Modules.notificationTemplateVariable.ToString(),
                Description= "Module for wallet notification template variable management",
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
