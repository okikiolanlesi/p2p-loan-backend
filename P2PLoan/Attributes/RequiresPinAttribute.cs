using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Constants;
using P2PLoan.Data;
using P2PLoan.Helpers;

namespace P2PLoan.Attributes;

public class RequiresPinAttribute : Attribute, IActionFilter
{
    private readonly P2PLoanDbContext dbContext;

    public RequiresPinAttribute(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;

        if (user == null || !user.Identity.IsAuthenticated)
        {
            // If user is not authenticated, return 401 Unauthorized
            context.Result = new UnauthorizedObjectResult(new ServiceResponse<object>(ResponseStatus.Unauthorized, AppStatusCodes.Unauthorized, "Unauthorized", null));
            return;
        }

        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            // If user is not authenticated, return 401 Unauthorized
            context.Result = new UnauthorizedObjectResult(new ServiceResponse<object>(ResponseStatus.Unauthorized, AppStatusCodes.Unauthorized, "Unauthorized", null));
            return;
        }

        var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);

        if (!dbUser.PinCreated)
        {
            // If user is not authenticated, return 401 Unauthorized
            context.Result = new BadRequestObjectResult(new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.NoPinCreated, "No Pin Created", null));
            return;
        }

    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // No action needed after the action execution in this case
    }
}
