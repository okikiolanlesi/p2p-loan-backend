using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;

namespace P2PLoan.Interfaces;

public interface IMonnifyService
{
    event Func<object, ManagedWalletCallbackDto<ManagedWalletCollectionCallbackData>, Task> CollectionProcessingCompleted;
    event Func<object, ManagedWalletCallbackDto<ManagedWalletDisbursementCallbackData>, Task> DisbursementProcessingCompleted;
    Task HandleDisbursementCallback(MonnifyCallbackDto<MonnifyDisbursementCallbackData> requestBody);
    Task HandleTransactionCompletionCallback(MonnifyCallbackDto<MonnifyCollectionCallBackData> requestBody);
}
