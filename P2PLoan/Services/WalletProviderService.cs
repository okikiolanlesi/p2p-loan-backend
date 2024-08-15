using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Constants;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Services;

public class WalletProviderService : IWalletProviderService
{
    private readonly IWalletProviderRepository _walletProviderRepository;

    public WalletProviderService(IWalletProviderRepository walletProviderRepository)
    {
        _walletProviderRepository = walletProviderRepository;
    }
    public async Task<ServiceResponse<object>> GetAllAsync()
    {
        var results = await _walletProviderRepository.GetAll();

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Wallet Providers fetched successfully", results);
    }
}
