using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Utils;

namespace P2PLoan.Services;

public class MonnifyService : IMonnifyService
{
    // Define the event
    public event Func<object, ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData>, Task> CollectionProcessingCompleted;
    public event Func<object, ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData>, Task> DisbursementProcessingCompleted;
    private readonly IManagedWalletRepository managedWalletRepository;
    private readonly IManagedWalletTransactionRepository managedWalletTransactionRepository;
    private readonly IManagedWalletTransactionTrackerRepository managedWalletTransactionTrackerRepository;
    private readonly IMapper mapper;
    private readonly IUserRepository userRepository;
    private readonly IMonnifyApiService monnifyApiService;
    private readonly IWalletTopUpDetailRepository walletTopUpDetailRepository;
    private readonly IWalletRepository walletRepository;

    public MonnifyService(IManagedWalletRepository managedWalletRepository, IManagedWalletTransactionRepository managedWalletTransactionRepository, IManagedWalletTransactionTrackerRepository managedWalletTransactionTrackerRepository, IMapper mapper, IUserRepository userRepository, IMonnifyApiService monnifyApiService, IWalletTopUpDetailRepository walletTopUpDetailRepository, IWalletRepository walletRepository)
    {
        this.managedWalletRepository = managedWalletRepository;
        this.managedWalletTransactionRepository = managedWalletTransactionRepository;
        this.managedWalletTransactionTrackerRepository = managedWalletTransactionTrackerRepository;
        this.mapper = mapper;
        this.userRepository = userRepository;
        this.monnifyApiService = monnifyApiService;
        this.walletTopUpDetailRepository = walletTopUpDetailRepository;
        this.walletRepository = walletRepository;
    }
    public async Task HandleDisbursementCallback(MonnifyCallbackDto<MonnifyDisbursementCallbackData> requestBody)
    {
        var transactionTracker = await managedWalletTransactionTrackerRepository.FindByInternalReferenceAsync(requestBody.EventData.Reference);

        if (transactionTracker == null)
        {
            // Means the transfer wasn't initiated by us
            return;
        }
        var systemUser = await userRepository.GetSystemUser();
        // Get the associated managed wallet used to initiate the transfer
        var managedWallet = await managedWalletRepository.GetByWalletReferenceAsync(transactionTracker.SourceAccountNumber);

        var sourceUser = await userRepository.GetByIdAsync(managedWallet.UserId);

        if (managedWallet == null)
        {
            throw new Exception("Managed wallet not found");
        }

        if (requestBody.EventType == "SUCCESSFUL_DISBURSEMENT")
        {
            // Update the ledger balance of the managed wallet, no need to update the available balance since the amount was already reserved
            managedWallet.LedgerBalance -= requestBody.EventData.Amount;

            managedWalletRepository.MarkAsModified(managedWallet);
            await managedWalletRepository.SaveChangesAsync();

            // Create a managed wallet transaction to track the disbursement
            var managedWalletTransaction = new ManagedWalletTransaction
            {
                Id = Guid.NewGuid(),
                ManagedWalletId = managedWallet.Id,
                Amount = requestBody.EventData.Amount,
                Fee = requestBody.EventData.Fee,
                TransactionReference = requestBody.EventData.Reference,
                TransactionDate = DateTime.UtcNow,
                Narration = requestBody.EventData.Narration,
                IsCredit = false,
                CreatedBy = systemUser,
                ModifiedBy = systemUser
            };

            managedWalletTransactionRepository.Add(managedWalletTransaction);

            await managedWalletTransactionRepository.SaveChangesAsync();
        }
        else if (requestBody.EventType == "FAILED_DISBURSEMENT")
        {
            // Update the available balance of the managed wallet since the disbursement failed
            managedWallet.AvailableBalance += requestBody.EventData.Amount;

            managedWalletRepository.MarkAsModified(managedWallet);
            await managedWalletRepository.SaveChangesAsync();

        }
        else if (requestBody.EventType == "REVERSED_DISBURSEMENT")
        {
            // Update the available balance of the managed wallet since the disbursement was reversed
            managedWallet.AvailableBalance += requestBody.EventData.Amount;
            // Update the ledger balance of the managed wallet since the disbursement was reversed
            managedWallet.LedgerBalance += requestBody.EventData.Amount;

            // Create a managed wallet transaction to track the disbursement reversal
            var managedWalletTransaction = new ManagedWalletTransaction
            {
                Id = Guid.NewGuid(),
                ManagedWalletId = managedWallet.Id,
                Amount = requestBody.EventData.Amount,
                Fee = requestBody.EventData.Fee,
                TransactionReference = requestBody.EventData.Reference,
                TransactionDate = DateTime.UtcNow,
                Narration = requestBody.EventData.Narration,
                IsCredit = true
            };

            managedWalletTransactionRepository.Add(managedWalletTransaction);
            await managedWalletTransactionRepository.SaveChangesAsync();
        }


        // The below steps are due to the fact that monnify fails to notify us about the transaction completion for disbursements into reserved accounts initiated by us so we have to manually trigger the event here as a workaround

        //Step 1: Get the managed destination wallet and user associated with the disbursement
        var topUpAccountDetail = await walletTopUpDetailRepository.FindByAccountNumberAndCode(requestBody.EventData.DestinationAccountNumber, requestBody.EventData.DestinationBankCode);



        MonnifyCallbackDto<MonnifyCollectionCallBackData> collectionCallbackData = null;

        // If topUpAccountDetail is null, it means the disbursement was not to a reserved account, so we don't need to notify P2P Loan about the transaction completion
        if (topUpAccountDetail != null)
        {
            var wallet = await walletRepository.FindById(topUpAccountDetail.WalletId);

            var managedDestinationWallet = await managedWalletRepository.GetByWalletReferenceAsync(wallet.ReferenceId);

            if (managedDestinationWallet == null)
            {
                throw new Exception("Managed wallet not found");
            }

            var recipientUser = await userRepository.GetByIdAsync(managedDestinationWallet.UserId);

            // Step 2: Create the MonnifyCallbackDto<MonnifyCollectionCallBackData> object to be passed to the HandleTransactionCompletionCallback method
            collectionCallbackData = new MonnifyCallbackDto<MonnifyCollectionCallBackData>
            {
                EventType = requestBody.EventType,
                EventData = new MonnifyCollectionCallBackData
                {
                    AmountPaid = requestBody.EventData.Amount,
                    TotalPayable = requestBody.EventData.Amount,
                    Currency = requestBody.EventData.Currency,
                    PaymentStatus = requestBody.EventType,
                    PaymentMethod = "ACCOUNT_TRANSFER",
                    PaymentReference = requestBody.EventData.Reference,
                    PaidOn = DateTime.UtcNow,
                    PaymentDescription = requestBody.EventData.Narration,
                    MetaData = new Dictionary<string, object>(),
                    TransactionReference = requestBody.EventData.TransactionReference + "-COLLECTION",

                    SettlementAmount = requestBody.EventData.Amount,
                    CardDetails = new CardDetails { },
                    Customer = new Customer
                    {
                        Email = recipientUser.Email,
                        Name = recipientUser.FirstName + " " + recipientUser.LastName,
                    },
                    Product = new Product
                    {
                        Reference = managedDestinationWallet.WalletReference,
                        Type = "RESERVED_ACCOUNT"
                    },
                    PaymentSourceInformation = new List<PaymentSourceInformation>
                {
                    new PaymentSourceInformation
                    {
                        AccountName = sourceUser.FirstName + " " + sourceUser.LastName,
                        AccountNumber = managedWallet.WalletReference,
                        AmountPaid = requestBody.EventData.Amount,
                        BankCode = "",
                        SessionId = RandomCharacterGenerator.GenerateRandomString(32)
                    }
                },
                    DestinationAccountInformation = new DestinationAccountInformation
                    {
                        AccountNumber = requestBody.EventData.DestinationAccountNumber,
                        BankCode = requestBody.EventData.DestinationBankCode,
                        BankName = requestBody.EventData.DestinationBankName
                    },
                }
            };


            // Step 3: Create a credit transaction using a dummy credit api I stumbled upon for the reserved account on monnify to reflect the collection, Note: This is a workaround to reflect the collection in the reserved account since monnify fails to notify us about the transaction completion for disbursements into reserved accounts initiated by us. Also note that this api only works in the sandbox environment
            var dummyCreditAccountRequestDto = new MonnifyDummyCreditAccountRequestDto
            {
                DestinationAccountNumber = collectionCallbackData.EventData.DestinationAccountInformation.AccountNumber,
                Amount = requestBody.EventData.Amount,
                SessionId = RandomCharacterGenerator.GenerateRandomString(32),
            };

            //The dummy credit account api requires a provider code to get the account to credit, I've created
            var providerCode = getMonnifyBankProviderCode(requestBody.EventData.DestinationBankCode);
            await monnifyApiService.DummyCreditAccount(dummyCreditAccountRequestDto, providerCode);
        }

        //Step 4: Notify P2P Loan of completed disbursement processing
        var managedWalletDisbursementCallbackDto = mapper.Map<ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData>>(requestBody);

        // Set the reference to the external reference of the transaction tracker so that P2P Loan can track the transaction
        managedWalletDisbursementCallbackDto.EventData.Reference = transactionTracker.ExternalReference;
        await OnDisbursementProcessingCompleted(managedWalletDisbursementCallbackDto);

        if (topUpAccountDetail != null && collectionCallbackData != null)
        {// Step 5: Call the HandleTransactionCompletionCallback method to notify P2P Loan of the transaction completion
            await HandleTransactionCompletionCallback(collectionCallbackData);
        }

        //That's all folks!
    }

    public async Task HandleTransactionCompletionCallback(MonnifyCallbackDto<MonnifyCollectionCallBackData> requestBody)
    {
        // Check if the transaction is a reserved account transaction and is not a transaction to be ignored, if not return because we are only interested in reserved account transactions 
        if (requestBody.EventData.Product.Type != "RESERVED_ACCOUNT" || requestBody.EventData.PaymentSourceInformation.Count == 0 || requestBody.EventData.PaymentSourceInformation[0].AccountNumber == "IGNORE")
        {
            return;
        }

        // Get the managed wallet associated with the transaction
        var managedWallet = await managedWalletRepository.GetByWalletReferenceAsync(requestBody.EventData.Product.Reference);

        if (managedWallet == null)
        {
            throw new Exception("Managed wallet not found");
        }

        // Update the available balance of the managed wallet
        managedWallet.AvailableBalance += requestBody.EventData.AmountPaid;
        // Update the ledger balance of the managed wallet
        managedWallet.LedgerBalance += requestBody.EventData.AmountPaid;

        managedWalletRepository.MarkAsModified(managedWallet);
        await managedWalletRepository.SaveChangesAsync();

        var systemUser = await userRepository.GetSystemUser();
        // Create a managed wallet transaction to track the transaction
        var managedWalletTransaction = new ManagedWalletTransaction
        {
            Id = Guid.NewGuid(),
            ManagedWalletId = managedWallet.Id,
            Amount = requestBody.EventData.AmountPaid,
            Fee = requestBody.EventData.AmountPaid,
            TransactionReference = requestBody.EventData.PaymentReference,
            TransactionDate = DateTime.UtcNow,
            Narration = requestBody.EventData.PaymentDescription,
            IsCredit = true,
            CreatedBy = systemUser,
            ModifiedBy = systemUser
        };

        managedWalletTransactionRepository.Add(managedWalletTransaction);

        await managedWalletTransactionRepository.SaveChangesAsync();

        // Notify P2P Loan of collection processing completion
        var managedWalletCollectionCallbackDto = mapper.Map<ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData>>(requestBody);

        // Set the reference to the external reference of the transaction tracker so that P2P Loan can track the transaction
        var transactionTracker = await managedWalletTransactionTrackerRepository.FindByInternalReferenceAsync(requestBody.EventData.PaymentReference);
        managedWalletCollectionCallbackDto.EventData.PaymentReference = transactionTracker.ExternalReference;

        await OnProcessingCollectionCompleted(managedWalletCollectionCallbackDto);
    }

    protected virtual async Task OnProcessingCollectionCompleted(ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData> managedWalletCollectionCallbackDto)
    {
        if (CollectionProcessingCompleted != null)
        {
            await CollectionProcessingCompleted.Invoke(this, managedWalletCollectionCallbackDto);
        }
    }

    protected virtual async Task OnDisbursementProcessingCompleted(ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData> managedWalletDisbursementCallbackDto)
    {
        if (DisbursementProcessingCompleted != null)
        {
            await DisbursementProcessingCompleted.Invoke(this, managedWalletDisbursementCallbackDto);
        }
    }

    // The below method is used to get the monnify bank provider code for the bank code, this helps the dummy credit account api to get the account to credit and is only used in the sandbox environment. I've created a mapping for the bank codes I know about, you can add more if you know more bank codes
    //TODO: remove this method when you are done testing the dummy credit account api
    private string getMonnifyBankProviderCode(string bankCode)
    {
        switch (bankCode)
        {
            case "035":
                return "81884";
            case "232":
                return "23284";
            case "044":
                return "04404";
            case "058":
                return "063";
            case "50515":
                return "063";
            default: return "000";
        }
    }
}