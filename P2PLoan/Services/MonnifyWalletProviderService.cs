using System;
using System.Threading.Tasks;
using AutoMapper;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Services;

public class MonnifyWalletProviderService : IThirdPartyWalletProviderService
{
    private readonly IMonnifyApiService monnifyApiService;
    private readonly IMapper mapper;

    public MonnifyWalletProviderService(IMonnifyApiService monnifyApiService, IMapper mapper)
    {
        this.monnifyApiService = monnifyApiService;
        this.mapper = mapper;
    }
    public async Task<CreateWalletResponseDto> Create(CreateWalletDto createWalletDto)
    {
        var createdWallet = await monnifyApiService.CreateWallet(createWalletDto);
        return mapper.Map<CreateWalletResponseDto>(createdWallet);
    }

    public async Task<GetBalanceResponseDto> GetBalance(Wallet wallet)
    {
        var walletBalance = await monnifyApiService.GetWalletBalance(wallet.AccountNumber);

        return mapper.Map<GetBalanceResponseDto>(walletBalance);
    }

    public async Task<GetTransactionsResponseDto> GetTransactions(Wallet wallet, int pageSize, int pageNo)
    {
        var walletTransactions = await monnifyApiService.GetWalletTransactions(wallet.AccountNumber, pageSize, pageNo);

        return mapper.Map<GetTransactionsResponseDto>(walletTransactions);
    }

    public async Task<TransferResponseDto> Transfer(TransferDto transferDto)
    {
        var response = await monnifyApiService.Transfer(mapper.Map<MonnifyTransferRequestBodyDto>(transferDto));

        return mapper.Map<TransferResponseDto>(response);
    }
}
