using System;
using Microsoft.AspNetCore.Mvc;

namespace P2PLoan.Helpers;

public class ControllerHelper
{
    public static IActionResult HandleApiResponse<T>(ServiceResponse<T> response)
    {
        return response.Status switch
        {
            ResponseStatus.Success => new OkObjectResult(response),
            ResponseStatus.Created => new ObjectResult(response)
            {
                StatusCode = 201,
            },
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
            ResponseStatus.Forbidden => new ObjectResult(response)
            {
                StatusCode = 403,
            },
            _ => new StatusCodeResult(500)
        };
    }
}
