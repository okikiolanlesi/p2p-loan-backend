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
    Task<MonnifyApiResponse<MonnifyVerifyAccountDetailsResponseBody>> VerifyAccountDetails(MonnifyVerifyAccountDetailsRequestDto verifyAccountDetailsRequestDto);
    Task<MonnifyApiResponse<List<BankDto>>> GetBanks();
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

public class MonnifyVerifyAccountDetailsRequestDto
{
    public string AccountNumber { get; set; }

   
    public string BankCode { get; set; }
}

public class MonnifyVerifyAccountDetailsResponseBody
{
    public string AccountNumber { get; set; }
    public string AccountName { get; set; }
    public string BankCode { get; set; }
}

public class MonnifyGetBanksResponse
{
    public List<BankDto> Banks { get; set; }
}

public class BankDto
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string UssdTemplate { get; set; }
    public string BaseUssdCode { get; set; }
    public string TransferUssdTemplate { get; set; }
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
