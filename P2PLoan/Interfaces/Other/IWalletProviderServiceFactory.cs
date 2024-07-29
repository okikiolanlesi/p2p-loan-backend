using P2PLoan.Helpers;

namespace P2PLoan.Interfaces
{
    public interface IWalletProviderServiceFactory
    {
        IWalletProviderService GetWalletProviderService(WalletProviders walletProvider);
    }
}
