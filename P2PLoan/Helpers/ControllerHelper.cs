using System;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Helpers;

namespace P2PLoan;

public class ControllerHelper
{
    public static IActionResult HandleApiResponse<T>(ServiceResponse<T> response)
    {
        return response.Status switch
        {
            ResponseStatus.Success => new OkObjectResult(response),
            ResponseStatus.Error => new ObjectResult(response)
            {
                StatusCode = 500,
            },
            ResponseStatus.NotFound => new NotFoundObjectResult(response),
            ResponseStatus.Unauthorized => new UnauthorizedObjectResult(response),
            ResponseStatus.Processing => new ObjectResult(response)
            {
                StatusCode = 102,
            },
            ResponseStatus.Accepted => new ObjectResult(response)
            {
                StatusCode = 202,
            },
            ResponseStatus.BadRequest => new BadRequestObjectResult(response),
            _ => new StatusCodeResult(500)
        };
    }
}
