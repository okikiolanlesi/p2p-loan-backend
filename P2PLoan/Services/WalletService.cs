using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository walletRepository;
        private readonly IWalletProviderServiceFactory walletProviderServiceFactory;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;

        public WalletService(IWalletRepository walletRepository, IWalletProviderServiceFactory walletProviderServiceFactory, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.walletRepository = walletRepository;
            this.walletProviderServiceFactory = walletProviderServiceFactory;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<object>> CreateWalletForController(WalletProviders walletProvider, CreateWalletDto createWalletDto, User user, Guid walletProviderId)
        {
            var providerService = walletProviderServiceFactory.GetWalletProviderService(walletProvider);

            if (providerService == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidProvider, "Invalid wallet provider", null);
            }

            var createWalletResponse = await providerService.Create(createWalletDto);
            if (createWalletResponse == null)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
            }

            var wallet = new Wallet
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                WalletProviderId = walletProviderId,
                AccountNumber = createWalletResponse.AccountNumber,
                ReferenceId = createWalletResponse.WalletReference,
                TopUpAccountName = createWalletResponse.TopUpAccountName,
                TopUpAccountNumber = createWalletResponse.TopUpAccountNumber,
                TopUpBankName = createWalletResponse.TopUpBankName,
                TopUpBankCode = createWalletResponse.TopUpBankCode,
            };

            walletRepository.Add(wallet);

            var result = await walletRepository.SaveChangesAsync();

            if (!result)
            {
                return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
            }

            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Wallet created successfully", wallet);
        }

        public async Task<CreateWalletResponse> Create(WalletProviders walletProvider, CreateWalletDto createWalletDto)
        {
            var walletProviderService = walletProviderServiceFactory.GetWalletProviderService(walletProvider);

            if (walletProviderService == null)
            {
                throw new Exception("Invalid wallet provider");
            }

            var response = await walletProviderService.Create(createWalletDto);
            if (response == null)
            {
                throw new Exception("Unable to create wallet");

            }
            var result = mapper.Map<CreateWalletResponse>(response);

            result.Created = true;

            return result;
        }

        public async Task<ServiceResponse<object>> GetBalanceForController(Guid walletId)
        {
            var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

            var wallet = await walletRepository.FindById(walletId);

            if (wallet == null || wallet.UserId != userId)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Wallet not found", null);
            }

            var providerService = walletProviderServiceFactory.GetWalletProviderService(wallet.WalletProvider.Slug);

            var balanceResponse = await providerService.GetBalance(wallet);
            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Balance fetched successfully", balanceResponse);
        }

        public async Task<GetBalanceResponseDto> GetBalance(Guid walletId)
        {
            var wallet = await walletRepository.FindById(walletId);

            if (wallet == null)
            {
                throw new Exception("Wallet not found");
            }

            var providerService = walletProviderServiceFactory.GetWalletProviderService(wallet.WalletProvider.Slug);

            var balanceResponse = await providerService.GetBalance(wallet);
            return balanceResponse;
        }

        public async Task<ServiceResponse<object>> GetTransactions(Guid walletId, int pageSize = 10, int pageNo = 1)
        {
            var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

            var wallet = await walletRepository.FindById(walletId);

            if (wallet == null || wallet.UserId != userId)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Wallet not found", null);
            }

            var providerService = walletProviderServiceFactory.GetWalletProviderService(wallet.WalletProvider.Slug);
            if (providerService == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidProvider, "Invalid wallet provider", null);
            }

            var transactionsResponse = await providerService.GetTransactions(wallet, pageSize, pageNo);

            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Transactions fetched successfully", transactionsResponse);
        }

        public async Task<ServiceResponse<object>> GetLoggedInUserWallets()
        {
            var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

            var wallets = await walletRepository.GetAllForAUser(userId);

            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Wallets fetched successfully", wallets);
        }

        public async Task<TransferResponseDto> Transfer(TransferDto transferDto, Wallet wallet)
        {
            transferDto.SourceAccountNumber = wallet.AccountNumber;

            var providerService = walletProviderServiceFactory.GetWalletProviderService(wallet.WalletProvider.Slug);

            if (providerService == null)
            {
                throw new Exception("Invalid wallet provider");
            }

            var response = await providerService.Transfer(transferDto);

            return response;
        }
    }
}
