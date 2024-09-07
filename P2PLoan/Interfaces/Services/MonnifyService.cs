using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;

namespace P2PLoan.Interfaces;

public interface IMonnifyService
{
    event EventHandler<ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData>> CollectionProcessingCompleted;
    event EventHandler<ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData>> DisbursementProcessingCompleted;
    Task HandleDisbursementCallback(MonnifyCallbackDto<MonnifyDisbursementCallbackData> requestBody);
    Task HandleTransactionCompletionCallback(MonnifyCallbackDto<MonnifyCollectionCallBackData> requestBody);
}
