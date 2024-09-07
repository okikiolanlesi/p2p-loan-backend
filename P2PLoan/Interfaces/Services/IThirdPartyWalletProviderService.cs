using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface IThirdPartyWalletProviderService
{
    Task<CreateWalletResponseDto> Create(CreateWalletDto createWalletDto);
    Task<GetBalanceResponseDto> GetBalance(Wallet wallet);
    Task<GetTransactionsResponseDto> GetTransactions(Wallet wallet, int pageSize, int pageNo);
    Task<TransferResponseDto> Transfer(TransferDto transferDto);
}
