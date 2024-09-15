using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Services;

public class ManagedWalletProviderService : IThirdPartyWalletProviderService
{
    private readonly IMonnifyApiService monnifyApiService;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;
    private readonly IManagedWalletRepository managedWalletRepository;
    private readonly IManagedWalletTransactionRepository managedWalletTransactionRepository;
    private readonly IManagedWalletTransactionTrackerRepository managedWalletTransactionTrackerRepository;
    private readonly IUserRepository userRepository;

    public ManagedWalletProviderService(IMonnifyApiService monnifyApiService, IMapper mapper, IConfiguration configuration, IManagedWalletRepository managedWalletRepository, IManagedWalletTransactionRepository managedWalletTransactionRepository, IManagedWalletTransactionTrackerRepository managedWalletTransactionTrackerRepository, IUserRepository userRepository)
    {
        this.monnifyApiService = monnifyApiService;
        this.mapper = mapper;
        this.configuration = configuration;
        this.managedWalletRepository = managedWalletRepository;
        this.managedWalletTransactionRepository = managedWalletTransactionRepository;
        this.managedWalletTransactionTrackerRepository = managedWalletTransactionTrackerRepository;
        this.userRepository = userRepository;
    }
    public async Task<CreateWalletResponseDto> Create(CreateWalletDto createWalletDto)
    {
        var createReservedAccountPayload = new MonnifyCreateReservedAccountRequestDto
        {
            AccountReference = createWalletDto.WalletReference,
            AccountName = createWalletDto.WalletName,
            CurrencyCode = "NGN",
            ContractCode = configuration["Monnify:ContractCode"],
            CustomerEmail = createWalletDto.CustomerEmail,
            CustomerName = createWalletDto.CustomerName,
            Bvn = createWalletDto.BvnDetails.Bvn,
            Nin = createWalletDto.Nin,
            GetAllAvailableBanks = true,
        };
        var createdReservedAccount = await monnifyApiService.CreateReservedAccount(createReservedAccountPayload);

        var managedWallet = new ManagedWallet
        {
            Id = Guid.NewGuid(),
            UserId = (Guid)createWalletDto.UserId,
            AvailableBalance = 0,
            LedgerBalance = 0,
            WalletReference = createdReservedAccount.ResponseBody.AccountReference,
            AccountName = createdReservedAccount.ResponseBody.AccountName,
            CreatedById = (Guid)createWalletDto.UserId,
            ModifiedById = (Guid)createWalletDto.UserId,
        };

        managedWalletRepository.Add(managedWallet);

        await managedWalletRepository.SaveChangesAsync();

        var payload = new CreateWalletResponseDto
        {
            WalletReference = createdReservedAccount.ResponseBody.AccountReference,
            AccountName = createdReservedAccount.ResponseBody.AccountName,
            CustomerName = createdReservedAccount.ResponseBody.CustomerName,
            CustomerEmail = createdReservedAccount.ResponseBody.CustomerEmail,
            BVN = createdReservedAccount.ResponseBody.Bvn,
            BVNDateOfBirth = createWalletDto.BvnDetails.BvnDateOfBirth,
            AccountNumber = createdReservedAccount.ResponseBody.AccountReference

        };

        var topUpAccountDetails = new List<TopUpAccountDetail>();

        foreach (Account topUpDetail in createdReservedAccount.ResponseBody.Accounts)
        {
            topUpAccountDetails.Add(new TopUpAccountDetail
            {
                AccountName = topUpDetail.AccountName,
                AccountNumber = topUpDetail.AccountNumber,
                BankCode = topUpDetail.BankCode,
                BankName = topUpDetail.BankName
            });
        }

        payload.TopUpAccountDetails = topUpAccountDetails;

        return payload;
    }

    public async Task<GetBalanceResponseDto> GetBalance(Wallet wallet)
    {
        var managedWallet = await managedWalletRepository.GetByWalletReferenceAsync(wallet.ReferenceId);

        return new GetBalanceResponseDto
        {
            AvailableBalance = managedWallet.AvailableBalance,
            LedgerBalance = managedWallet.LedgerBalance
        };
    }

    public async Task<GetTransactionsResponseDto> GetTransactions(Wallet wallet, int pageSize, int pageNo)
    {
        var managedWallet = await managedWalletRepository.GetByWalletReferenceAsync(wallet.ReferenceId);

        var transactions = await managedWalletTransactionRepository.GetTransactionsByWalletId(managedWallet.Id, pageSize, pageNo);

        var totalPages = (int)Math.Ceiling((decimal)transactions.TotalItems / pageSize);
        var response = new GetTransactionsResponseDto
        {
            Content = transactions.Items.Select(t => new Transaction
            {
                Id = t.Id,
                Amount = t.Amount,
                IsCredit = t.IsCredit,
                Narration = t.Narration,
                TransactionDate = t.TransactionDate,
                TransactionReference = t.TransactionReference
            }).ToList(),
            TotalPages = totalPages,
            TotalElements = transactions.TotalItems,
            NumberOfElements = transactions.Items.Count(),
            Size = pageSize,
            Number = pageNo,
            Empty = !transactions.Items.Any()
        };
        return response;
    }

    public async Task<TransferResponseDto> Transfer(TransferDto transferDto)
    {
        var systemUser = await userRepository.GetSystemUser();
        var payload = mapper.Map<MonnifyTransferRequestBodyDto>(transferDto);
        payload.Async = true;

        // Track transaction for managed wallet, this will help keep track of the reference passed from the client
        var transactionTracker = new ManagedWalletTransactionTracker
        {
            Id = Guid.NewGuid(),
            InternalReference = Guid.NewGuid().ToString(),
            ExternalReference = payload.Reference,
            DestinationAccountNumber = payload.DestinationAccountNumber,
            DestinationBankCode = payload.DestinationBankCode,
            SourceAccountNumber = payload.SourceAccountNumber,
            Status = "PENDING",
            CreatedBy = systemUser,
            ModifiedBy = systemUser
        };
        var managedWallet = await managedWalletRepository.GetByWalletReferenceAsync(payload.SourceAccountNumber);

        if (managedWallet == null)
        {
            throw new Exception("Managed wallet not found");
        }

        // Update the available balance of the managed wallet pending the completion of the transaction
        // This is to ensure that the managed wallet balance is updated immediately the transaction is initiated
        // And then updated again when the transaction is completed so that the managed wallet balance is always in sync with the actual balance
        managedWallet.AvailableBalance -= payload.Amount;

        // Update the managed wallet in the database
        managedWalletRepository.MarkAsModified(managedWallet);
        await managedWalletRepository.SaveChangesAsync();


        // Save the transaction tracker to the database
        managedWalletTransactionTrackerRepository.Add(transactionTracker);
        await managedWalletTransactionRepository.SaveChangesAsync();


        // Replace the source account number with the source account number from the configuration, cause we want to transfer from the source account number in the configuration and then manage the balance and transaction tracking internally
        payload.SourceAccountNumber = configuration["Monnify:SourceAccountNumber"];

        // Replace the reference with the internal reference generated for the transaction tracker
        payload.Reference = transactionTracker.InternalReference;

        // Transfer the amount from the source account number in the configuration to the destination account number
        var response = await monnifyApiService.Transfer(mapper.Map<MonnifyTransferRequestBodyDto>(payload));

        return mapper.Map<TransferResponseDto>(response);
    }
}
