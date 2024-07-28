using System;
using System.Threading.Tasks;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;

namespace P2PLoan;

public class WalletService : IWalletService
{

    public IWalletProviderService walletProviderServiceRepository { get; set; }
    public IWalletRepository walletRepository { get; }
    public WalletService(IWalletRepository walletRepository)
    {
        this.walletRepository = walletRepository;
    }



    public Task<ApiResponse<object>> Create(WalletProviders walletProvider, CreateWalletDto createWalletDto, bool forController)
    {
        throw new NotImplementedException();
    }

    Task<CreateWalletResponse> IWalletService.Create(WalletProviders walletProvider, CreateWalletDto createWalletDto)
    {
        setProviderInstance(walletProvider);
        throw new NotImplementedException();
    }

    public Task<ApiResponse<object>> GetBalance()
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<object>> GetBalance(WalletProviders walletProvider, string accountNumber)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<object>> GetTransactions(WalletProviders walletProvider, string accountNumber)
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<object>> Transfer()
    {
        throw new NotImplementedException();
    }

    public Task<ApiResponse<object>> Transfer(WalletProviders walletProvider, string accountNumber)
    {
        throw new NotImplementedException();
    }

    private void setProviderInstance(WalletProviders walletProvider)
    {
        switch (walletProvider)
        {
            case WalletProviders.monnify:
                walletProviderServiceRepository = new MonnifyWalletProviderService();
                break;
            default:
                walletProviderServiceRepository = new MonnifyWalletProviderService();
                break;
        }
    }

}
