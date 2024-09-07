using System;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders;

public class P_7_DisableMonnifyWalletProvider : ISeeder
{
    private readonly IWalletProviderRepository walletProviderRepository;
    private readonly IUserRepository userRepository;

    public P_7_DisableMonnifyWalletProvider(IWalletProviderRepository walletProviderRepository, IUserRepository userRepository)
    {
        this.walletProviderRepository = walletProviderRepository;
        this.userRepository = userRepository;
    }
    public async Task up()
    {
        var systemUser = await userRepository.GetSystemUser();
        var monnifyWalletProvider = await walletProviderRepository.FindBySlug(WalletProviders.monnify);
        monnifyWalletProvider.Enabled = false;
        monnifyWalletProvider.ModifiedBy = systemUser;
        walletProviderRepository.MarkAsModified(monnifyWalletProvider);
        await walletProviderRepository.SaveChangesAsync();
    }
    public async Task down()
    {
        throw new NotImplementedException();
    }
    public string Description()
    {
        return "Disable monnify wallet provider, be cause the service is currently not available";
    }
}
