using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Attributes;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/wallet")]
public class WalletController
{
    private readonly IWalletService walletService;

    public WalletController(IWalletService walletService)
    {
        this.walletService = walletService;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var response = await walletService.GetLoggedInUserWallets();
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("balance/{walletId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetBalance(Guid walletId)
    {
        var response = await walletService.GetBalanceForController(walletId);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("transactions/{walletId:guid}")]
    [Authorize]
    public async Task<IActionResult> GetTransactions(Guid walletId, [FromQuery] int pageSize = 10, [FromQuery] int pageNo = 1)
    {
        var response = await walletService.GetTransactions(walletId, pageSize, pageNo);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet("withdrawal/fee/{amount:double}")]
    [Authorize]
    public async Task<IActionResult> GetWithdrawalFee(double amount)
    {
        var response = await walletService.GetWithdrawalFee(amount);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost("withdraw")]
    [Authorize]
    [TypeFilter(typeof(RequiresPinAttribute))]
    public async Task<IActionResult> Withdraw([FromBody] WithdrawRequestDto withdrawRequestDto)
    {
        var response = await walletService.Withdraw(withdrawRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }
}
