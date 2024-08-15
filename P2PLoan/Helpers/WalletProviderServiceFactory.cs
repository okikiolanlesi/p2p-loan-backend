using System;
using Microsoft.Extensions.DependencyInjection;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;

namespace P2PLoan.Helpers
{
    public class WalletProviderServiceFactory : IWalletProviderServiceFactory
    {
        private readonly IServiceProvider serviceProvider;

        public WalletProviderServiceFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IThirdPartyWalletProviderService GetWalletProviderService(WalletProviders walletProvider)
        {
            return walletProvider switch
            {
                WalletProviders.monnify => serviceProvider.GetService<MonnifyWalletProviderService>(),
                _ => throw new Exception("Invalid wallet provider"),
            };
        }
    }
}
