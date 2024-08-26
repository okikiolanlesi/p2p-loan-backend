using System;
using Microsoft.AspNetCore.Mvc;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/monnify")]
public class MonnifyController
{

    [HttpPost("/disbursement/callback")]
    public async Task<IActionResult> CreateModule([FromBody] CreateModuleRequestDto createModuleRequestDto)
    {
        var response = await moduleService.CreateModuleAsync(createModuleRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

}
