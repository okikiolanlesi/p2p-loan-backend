using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;

namespace P2PLoan.Interfaces;

public interface IWalletProviderService
{
    Task<CreateWalletResponseDto> Create(CreateWalletDto createWalletDto);
    Task<GetBalanceResponseDto> GetBalance(string walletUniqueReference);
    Task<GetTransactionsResponseDto> GetTransactions(string walletUniqueReference, int pageSize, int pageNo);
}
