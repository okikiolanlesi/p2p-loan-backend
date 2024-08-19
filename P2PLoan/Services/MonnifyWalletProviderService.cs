using System;
using System.Threading.Tasks;
using AutoMapper;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;

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

    public async Task<GetBalanceResponseDto> GetBalance(string walletUniqueReference)
    {
        var walletBalance = await monnifyApiService.GetWalletBalance(walletUniqueReference);

        return mapper.Map<GetBalanceResponseDto>(walletBalance);
    }

    public async Task<GetTransactionsResponseDto> GetTransactions(string walletUniqueReference, int pageSize, int pageNo)
    {
        var walletTransactions = await monnifyApiService.GetWalletTransactions(walletUniqueReference, pageSize, pageNo);

        return mapper.Map<GetTransactionsResponseDto>(walletTransactions);
    }

    public Task<GetTransactionsResponseDto> Transfer(string walletUniqueReference, int pageSize, int pageNo)
    {
        throw new NotImplementedException();
    }
}
