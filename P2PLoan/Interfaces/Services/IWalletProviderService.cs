using System;
using System.Threading.Tasks;

namespace P2PLoan;

public interface IWalletProviderService
{
    Task<CreateWalletResponseDto> Create(CreateWalletDto createWalletDto);
    Task<GetBalanceResponseDto> GetBalance(string walletUniqueReference);
    Task<GetTransactionsResponseDto> GetTransactions(string walletUniqueReference, int pageSize, int pageNo);
}
