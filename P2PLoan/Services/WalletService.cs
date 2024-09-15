using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Utils;

namespace P2PLoan.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository walletRepository;
        private readonly IWalletProviderServiceFactory walletProviderServiceFactory;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IWalletTopUpDetailRepository walletTopUpDetailRepository;
        private readonly IUserRepository userRepository;
        private readonly IPaymentReferenceRepository paymentReferenceRepository;
        private readonly IConfiguration configuration;
        private readonly IConstants constants;

        public WalletService(IWalletRepository walletRepository, IWalletProviderServiceFactory walletProviderServiceFactory, IMapper mapper, IHttpContextAccessor httpContextAccessor, IWalletTopUpDetailRepository walletTopUpDetailRepository, IConstants constants, IUserRepository userRepository, IPaymentReferenceRepository paymentReferenceRepository, IConfiguration configuration)
        {
            this.walletRepository = walletRepository;
            this.walletProviderServiceFactory = walletProviderServiceFactory;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.walletTopUpDetailRepository = walletTopUpDetailRepository;
            this.constants = constants;
            this.userRepository = userRepository;
            this.paymentReferenceRepository = paymentReferenceRepository;
            this.configuration = configuration;
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

                    await transaction.CommitAsync();
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

        public async Task<ServiceResponse<object>> Withdraw(WithdrawRequestDto withdrawRequestDto)
        {
            var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User not found", null);
            }

            var wallet = await walletRepository.FindById(withdrawRequestDto.WalletId);

            if (wallet == null || wallet.UserId != userId)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Wallet not found", null);
            }

            var walletBalance = await GetBalance(withdrawRequestDto.WalletId);

            if (walletBalance.AvailableBalance < withdrawRequestDto.Amount)
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InsufficientFunds, "You do not have enough balance for the withdrawal", null);
            }

            var fee = Utilities.CalculateFee(withdrawRequestDto.Amount, constants.WITHDRAWAL_FEE_PERCENTAGE, constants.WITHDRAWAL_FEE_CAP);

            var systemUser = await userRepository.GetSystemUser();

            //Create new payment reference to track the withdrawal
            var paymentReferenceId = Guid.NewGuid();
            var paymentReference = new PaymentReference
            {
                Id = paymentReferenceId,
                ResourceId = wallet.Id,
                paymentReferenceType = PaymentReferenceType.Withdrawal,
                Reference = paymentReferenceId.ToString(),
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            };

            paymentReferenceRepository.Add(paymentReference);

            await paymentReferenceRepository.SaveChangesAsync();
            // Try to transfer the funds from the  wallet
            var transferDto = new TransferDto
            {
                Amount = withdrawRequestDto.Amount - fee,
                Reference = paymentReference.Id.ToString(),
                Narration = $"Withdrawal from wallet  {wallet.Id}",
                DestinationBankCode = withdrawRequestDto.DestinationBankCode,
                DestinationAccountNumber = withdrawRequestDto.DestinationAccountNumber,
                SourceAccountNumber = wallet.AccountNumber,
            };

            try
            {
                var transferResponse = await Transfer(transferDto, wallet);
            }
            catch
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InternalServerError, "Failed to debit your  wallet", null);
            }

            try
            {
                transferDto.Amount = fee;
                transferDto.Narration = $"Withdrawal fee from wallet  {wallet.Id}";
                transferDto.Reference = paymentReference.Id.ToString();
                transferDto.DestinationAccountNumber = configuration["FeeCollectionAccount:AccountNumber"];
                transferDto.DestinationBankCode = configuration["FeeCollectionAccount:BankCode"];

                var transferResponse = await Transfer(transferDto, wallet);
            }
            catch
            {
                return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InternalServerError, "Failed to debit your  wallet", null);
            }

            return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Withdrawal successful", null);
        }

        public Task<ServiceResponse<object>> GetWithdrawalFee(double amount)
        {
            var fee = Utilities.CalculateFee(amount, constants.WITHDRAWAL_FEE_PERCENTAGE, constants.WITHDRAWAL_FEE_CAP);

            return Task.FromResult(new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Fee calculated successfully", new { fee }));
        }
    }
}
