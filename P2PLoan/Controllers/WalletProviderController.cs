using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/wallet-provider")]
public class WalletProviderController
{
    private readonly IWalletProviderService walletProviderService;

    public WalletProviderController(IWalletProviderService walletProviderService)
    {
        this.walletProviderService = walletProviderService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var response = await walletProviderService.GetAllAsync();
        return ControllerHelper.HandleApiResponse(response);
    }
}
