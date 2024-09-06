using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs;

namespace P2PLoan.Interfaces;

public interface IMonnifyApiService
{
    Task<MonnifyApiResponse<MonnifyCreateWalletResponseBody>> CreateWallet(CreateWalletDto createWalletDto);
    Task<MonnifyApiResponse<MonnifyGetBalanceResponseBody>> GetWalletBalance(string walletUniqueReference);
    Task<MonnifyApiResponse<MonnifyGetTransactionsResponseBody>> GetWalletTransactions(string accountNumber, int pageSize = 10, int pageNo = 1);
    Task<MonnifyApiResponse<MonnifyGetSingleTransferResponseBody>> Transfer(MonnifyTransferRequestBodyDto transferDto);
    Task<MonnifyApiResponse<MonnifyVerifyBVNResponseBody>> VerifyBVN(MonnifyVerifyBVNRequestDto verifyBVNDto);
    Task<MonnifyApiResponse<MonnifyCreateReservedAccountResponseBody>> CreateReservedAccount(MonnifyCreateReservedAccountRequestDto createReservedAccountDto);
}

public class MonnifyCreateReservedAccountRequestDto
{
    public string AccountReference { get; set; }
    public string AccountName { get; set; }
    public string CurrencyCode { get; set; }
    public string ContractCode { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerName { get; set; }
    public string Bvn { get; set; }
    public string Nin { get; set; }
    public bool GetAllAvailableBanks { get; set; }
}


public class MonnifyCreateReservedAccountResponseBody
{
    public string ContractCode { get; set; }
    public string AccountReference { get; set; }
    public string AccountName { get; set; }
    public string CurrencyCode { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerName { get; set; }
    public List<Account> Accounts { get; set; }
    public string CollectionChannel { get; set; }
    public string ReservationReference { get; set; }
    public string ReservedAccountType { get; set; }
    public string Status { get; set; }
    public DateTime CreatedOn { get; set; }
    public List<IncomeSplitConfig> IncomeSplitConfig { get; set; }
    public string Bvn { get; set; }
    public bool RestrictPaymentSource { get; set; }
}

public class Account
{
    public string BankCode { get; set; }
    public string BankName { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
}

public class IncomeSplitConfig
{
    // Define properties for income split configuration if needed
}

public class MonnifyVerifyBVNRequestDto
{
    public string Bvn { get; set; }
    public string Name { get; set; }
    public string DateOfBirth { get; set; }
    public string MobileNo { get; set; }
}

public class MonnifyVerifyBVNResponseBody
{
    public string BVN { get; set; }
    public Match Name { get; set; }
    public string DateOfBirth { get; set; }
    public string MobileNo { get; set; }
}

public class Match
{
    public string MatchStatus { get; set; }
    public int MatchPercentage { get; set; }
}

public class MonnifyTransferResponseBody : CreateWalletDto
{
    public string FeeBearer { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public TopUpAccountDetails TopUpAccountDetails { get; set; }
}
public class MonnifyCreateWalletResponseBody : CreateWalletDto
{
    public string FeeBearer { get; set; }
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public TopUpAccountDetails TopUpAccountDetails { get; set; }
}

public class TopUpAccountDetails
{
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public string BankCode { get; set; }
    public string BankName { get; set; }
}
public class MonnifyGetBalanceResponseBody
{
    public double AvailableBalance { get; set; }
    public double LedgerBalance { get; set; }
}

public class MonnifyGetSingleTransferResponseBody
{
    public double Amount { get; set; }
    public string Reference { get; set; }
    public string Status { get; set; }
    public DateTime DateCreated { get; set; }
    public double TotalFee { get; set; }

    public string DestinationBankCode { get; set; }
    public string DestinationBankName { get; set; }
    public string DestinationAccountNumber { get; set; }
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
