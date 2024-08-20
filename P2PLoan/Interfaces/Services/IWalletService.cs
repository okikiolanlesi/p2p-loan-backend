using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IWalletService
{
    Task<CreateWalletResponse> Create(WalletProviders walletProvider, CreateWalletDto createWalletDto);
    Task<ServiceResponse<object>> CreateWalletForController(WalletProviders walletProvider, CreateWalletDto createWalletDto, User user, Guid walletProviderId);
    Task<ServiceResponse<object>> GetBalanceForController(Guid walletId);
    Task<GetBalanceResponseDto> GetBalance(Guid walletId);
    Task<ServiceResponse<object>> GetTransactions(Guid walletId, int pageSize = 10, int pageNo = 1);
    Task<ServiceResponse<object>> Transfer(WalletProviders walletProvider, string accountNumber);
    Task<ServiceResponse<object>> GetLoggedInUserWallets();
}

public class CreateWalletResponse
{
    public bool Created { get; set; }
    public string Message { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public string WalletReference { get; set; }
    public string TopUpAccountNumber { get; set; }
    public string TopUpAccountName { get; set; }
    public string TopUpBankCode { get; set; }
    public string TopUpBankName { get; set; }
}