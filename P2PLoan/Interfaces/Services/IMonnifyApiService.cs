using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P2PLoan;

public interface IMonnifyApiService
{
    Task<MonnifyApiResponse<MonnifyCreateWalletResponseBody>> CreateWallet(CreateWalletDto createWalletDto);
    Task<MonnifyApiResponse<MonnifyGetBalanceResponseBody>> GetWalletBalance(string walletUniqueReference);
    Task<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>> GetWalletTransactions(string accountNumber);
}

public class MonnifyCreateWalletResponseBody : CreateWalletDto
{
    public string FeeBearer { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
}

public class MonnifyGetBalanceResponseBody
{
    public int AvailableBalance { get; set; }
    public int LedgerBalance { get; set; }
}

public class MonnifyGetTransactionsResponseBody
{
    public List<object> Content { get; set; }
    public Pageable Pageable { get; set; }
    public bool Last { get; set; }
    public int TotalPages { get; set; }
    public int TotalElements { get; set; }
    public Sort Sort { get; set; }
    public bool First { get; set; }
    public int NumberOfElements { get; set; }
    public int Size { get; set; }
    public int Number { get; set; }
    public bool Empty { get; set; }
}

public class Pageable
{
    public Sort Sort { get; set; }
    public int PageSize { get; set; }
    public int PageNumber { get; set; }
    public int Offset { get; set; }
    public bool Paged { get; set; }
    public bool Unpaged { get; set; }
}

public class Sort
{
    public bool Sorted { get; set; }
    public bool Unsorted { get; set; }
    public bool Empty { get; set; }
}
