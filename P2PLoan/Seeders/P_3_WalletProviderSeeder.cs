using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders;

public class P_2_WalletProviderSeeder : ISeeder
{
    private readonly IWalletProviderRepository walletProviderRepository;
    private readonly IUserRepository userRepository;

    public P_2_WalletProviderSeeder(IWalletProviderRepository walletProviderRepository, IUserRepository userRepository)
    {
        this.walletProviderRepository = walletProviderRepository;
        this.userRepository = userRepository;
    }
    public async Task up()
    {
        var systemUser = await userRepository.GetSystemUser();
        var walletProviders = new List<WalletProvider>{
            new WalletProvider{
                Name= "Momo",
                Description= "MOMO wallet provider",
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            },

        };

        // WalletProviderRepository.AddRange(walletProviders);

        // await WalletProviderRepository.SaveChangesAsync();
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
