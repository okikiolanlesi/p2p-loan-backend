using System;
using System.Threading.Tasks;
using AutoMapper;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Services;

public class MonnifyService : IMonnifyService
{
    // Define the event
    public event EventHandler<ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData>> CollectionProcessingCompleted;
    public event EventHandler<ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData>> DisbursementProcessingCompleted;
    private readonly IManagedWalletRepository managedWalletRepository;
    private readonly IManagedWalletTransactionRepository managedWalletTransactionRepository;
    private readonly IManagedWalletTransactionTrackerRepository managedWalletTransactionTrackerRepository;
    private readonly IMapper mapper;
    private readonly IUserRepository userRepository;

    public MonnifyService(IManagedWalletRepository managedWalletRepository, IManagedWalletTransactionRepository managedWalletTransactionRepository, IManagedWalletTransactionTrackerRepository managedWalletTransactionTrackerRepository, IMapper mapper, IUserRepository userRepository)
    {
        this.managedWalletRepository = managedWalletRepository;
        this.managedWalletTransactionRepository = managedWalletTransactionRepository;
        this.managedWalletTransactionTrackerRepository = managedWalletTransactionTrackerRepository;
        this.mapper = mapper;
        this.userRepository = userRepository;
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

        //Notify P2P Loan of completed disbursement processing
        var managedWalletCollectionCallbackDto = mapper.Map<ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData>>(requestBody);

        // Set the reference to the external reference of the transaction tracker so that P2P Loan can track the transaction
        managedWalletCollectionCallbackDto.EventData.Reference = transactionTracker.ExternalReference;

        OnDisbursementProcessingCompleted(managedWalletCollectionCallbackDto);
    }

    public async Task HandleTransactionCompletionCallback(MonnifyCallbackDto<MonnifyCollectionCallBackData> requestBody)
    {
        // Check if the transaction is a reserved account transaction, if not return because we are only interested in reserved account transactions
        if (requestBody.EventData.Product.Type != "RESERVED_ACCOUNT")
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
        managedWallet.AvailableBalance += requestBody.EventData.TotalPayable;
        // Update the ledger balance of the managed wallet
        managedWallet.LedgerBalance += requestBody.EventData.TotalPayable;

        managedWalletRepository.MarkAsModified(managedWallet);
        await managedWalletRepository.SaveChangesAsync();

        var systemUser = await userRepository.GetSystemUser();
        // Create a managed wallet transaction to track the transaction
        var managedWalletTransaction = new ManagedWalletTransaction
        {
            Id = Guid.NewGuid(),
            ManagedWalletId = managedWallet.Id,
            Amount = requestBody.EventData.SettlementAmount,
            Fee = requestBody.EventData.TotalPayable - requestBody.EventData.SettlementAmount,
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
        OnProcessingCollectionCompleted(managedWalletCollectionCallbackDto);
    }

    protected virtual void OnProcessingCollectionCompleted(ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData> managedWalletCollectionCallbackDto)
    {
        CollectionProcessingCompleted?.Invoke(this, managedWalletCollectionCallbackDto);
    }
    protected virtual void OnDisbursementProcessingCompleted(ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData> managedWalletDisbursementCallbackDto)
    {
        DisbursementProcessingCompleted?.Invoke(this, managedWalletDisbursementCallbackDto);
    }
}