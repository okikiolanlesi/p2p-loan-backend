using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders;

public class P_6_PersonalWalletProviderSeeder : ISeeder
{
    private readonly IWalletProviderRepository walletProviderRepository;
    private readonly IUserRepository userRepository;

    public P_6_PersonalWalletProviderSeeder(IWalletProviderRepository walletProviderRepository, IUserRepository userRepository)
    {
        this.walletProviderRepository = walletProviderRepository;
        this.userRepository = userRepository;
    }
    public async Task up()
    {
        var systemUser = await userRepository.GetSystemUser();
        var walletProviders = new List<WalletProvider>{
            new WalletProvider{
                Name="Borrow Hub",
                Description= "Borrow Hub wallet provider",
                Slug = WalletProviders.borrowHub,
                Enabled = true,
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },
        };

        walletProviderRepository.AddRange(walletProviders);

        await walletProviderRepository.SaveChangesAsync();
    }
    public async Task down()
    {
        throw new NotImplementedException();
    }
    public string Description()
    {
        return "New wallet";
    }
}
