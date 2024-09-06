using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Attributes;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;

namespace P2PLoan.Controllers;

// This controller handles all Monnify related callbacks which we leverage on for our managed wallet transactions
[ApiController]
[Route("api/monnify")]
// [TypeFilter(typeof(ValidateMonnifySignatureAttribute))]
public class MonnifyController : ControllerBase
{
    private readonly IMonnifyService monnifyService;

    public MonnifyController(IMonnifyService monnifyService, IManagedWalletCallbackHandler managedWalletCallbackHandler)
    {
        this.monnifyService = monnifyService;
        managedWalletCallbackHandler.Subscribe(monnifyService);
    }

    [HttpPost("transaction-completion/callback")]
    public async Task<IActionResult> HandleTransactionCompletionCallback([FromBody] MonnifyCallbackDto<MonnifyCollectionCallBackData> requestBody)
    {
        await monnifyService.HandleTransactionCompletionCallback(requestBody);
        return new OkResult();
    }

    [HttpPost("disbursement/callback")]
    public async Task<IActionResult> HandleDisbursementCallback([FromBody] MonnifyCallbackDto<MonnifyDisbursementCallbackData> requestBody)
    {
        await monnifyService.HandleDisbursementCallback(requestBody);
        return new OkResult();
    }
}