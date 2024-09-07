using System;
using System.Collections.Generic;
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
        private readonly IWalletTopUpDetailRepository walletTopUpDetailRepository;

        public WalletService(IWalletRepository walletRepository, IWalletProviderServiceFactory walletProviderServiceFactory, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWalletTopUpDetailRepository walletTopUpDetailRepository)
        {
            this.walletRepository = walletRepository;
            this.walletProviderServiceFactory = walletProviderServiceFactory;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.walletTopUpDetailRepository = walletTopUpDetailRepository;
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

            var walletId = Guid.NewGuid();
            var topUpDetails = new List<WalletTopUpDetail>();
            foreach (var topUpDetail in createWalletResponse.TopUpAccountDetails)
            {
                topUpDetails.Add(new WalletTopUpDetail
                {
                    AccountName = topUpDetail.AccountName,
                    AccountNumber = topUpDetail.AccountNumber,
                    BankCode = topUpDetail.BankCode,
                    BankName = topUpDetail.AccountName,
                    WalletId = walletId,
                    Id = Guid.NewGuid()
                });
            }
            var wallet = new Wallet
            {
                Id = walletId,
                UserId = user.Id,
                WalletProviderId = walletProviderId,
                AccountNumber = createWalletResponse.AccountNumber,
                ReferenceId = createWalletResponse.WalletReference,
                // TopUpDetails = topUpDetails,
            };

            using (var transaction = await walletRepository.BeginTransactionAsync())
            {
                try
                {


                    walletRepository.Add(wallet);

                    var result = await walletRepository.SaveChangesAsync();

                    if (!result)
                    {
                        throw new Exception("Unable to save wallet");
                    }

                    walletTopUpDetailRepository.AddRange(topUpDetails);
                    var saveTopUpDetailsResult = await walletTopUpDetailRepository.SaveChangesAsync();

                    if (!saveTopUpDetailsResult)
                    {
                        throw new Exception("Unable to save wallet top up details");
                    }
                }
                catch (Exception ex)
                {
                    // Rollback the transaction if any error occurs
                    await transaction.RollbackAsync();
                    Console.WriteLine(ex);
                    return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "An unexpected error occurred", null);
                }

                return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Wallet created successfully", wallet);
            }
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
