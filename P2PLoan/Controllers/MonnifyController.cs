using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Helpers;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/monnify")]
public class MonnifyController
{

    [HttpPost("/disbursement/callback")]
    public async Task<IActionResult> HandleDisbursementCallback()
    {
        await Task.Delay(1000);
        // return ControllerHelper.HandleApiResponse(response);
        return new OkResult();
    }

}
