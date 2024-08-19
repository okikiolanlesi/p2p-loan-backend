using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces
{
    public interface IWalletProviderServiceFactory
    {
        IThirdPartyWalletProviderService GetWalletProviderService(WalletProviders walletProvider);
    }
}
