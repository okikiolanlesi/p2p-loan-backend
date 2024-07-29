using System;
using Microsoft.Extensions.DependencyInjection;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;

namespace P2PLoan
{
    public class WalletProviderServiceFactory : IWalletProviderServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public WalletProviderServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IWalletProviderService GetWalletProviderService(WalletProviders walletProvider)
        {
            return walletProvider switch
            {
                WalletProviders.monnify => serviceProvider.GetService<MonnifyWalletProviderService>(),
                _ => throw new Exception("Invalid wallet provider"),
            };
        }
    }
}
