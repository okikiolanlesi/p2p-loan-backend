using System.Threading.Tasks;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces;

public interface IWalletService
{

    Task<ApiResponse<object>> Create(WalletProviders walletProvider, CreateWalletDto createWalletDto, bool forController);
    Task<CreateWalletResponse> Create(WalletProviders walletProvider, CreateWalletDto createWalletDto);
    Task<ApiResponse<object>> GetBalance(WalletProviders walletProvider, string accountNumber);
    Task<ApiResponse<object>> GetTransactions(WalletProviders walletProvider, string accountNumber);
    Task<ApiResponse<object>> Transfer(WalletProviders walletProvider, string accountNumber);
}

public class CreateWalletResponse
{
    public bool Created { get; set; }
    public string Message { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public string WalletReference { get; set; }
}