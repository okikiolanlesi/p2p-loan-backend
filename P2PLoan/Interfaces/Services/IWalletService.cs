using System;
using System.Collections;
using System.Collections.Generic;
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
    Task<TransferResponseDto> Transfer(TransferDto transferDto, Wallet wallet);
    Task<ServiceResponse<object>> GetLoggedInUserWallets();
}

public class CreateWalletResponse
{
    public bool Created { get; set; }
    public string Message { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public string WalletReference { get; set; }
    public IEnumerable<TopUpAccountDetail> TopUpAccountDetails { get; set; }
}

public class TransferResponseDto
{
    public double Amount { get; set; }
    public string Reference { get; set; }
    public string Status { get; set; }
    public double TotalFee { get; set; }
    public DateTime DateCreated { get; set; }
    public string DestinationBankCode { get; set; }
    public string? DestinationBankName { get; set; }
    public string DestinationAccountNumber { get; set; }
}

// "responseBody": {
//         "amount": 200,
//         "reference": "referen00ce---1290034",
//         "status": "SUCCESS",
//         "dateCreated": "2022-07-31T14:31:33.759+0000",
//         "totalFee": 35,
//         "destinationAccountName": "BENJAMIN CHUKWUEMEKA ONONOGBU",
//         "destinationBankName": "Zenith bank",
//         "destinationAccountNumber": "2085886393",
//         "destinationBankCode": "057"
//     }