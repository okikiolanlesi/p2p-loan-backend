using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Constants;
using P2PLoan.Data;
using P2PLoan.Helpers;

namespace P2PLoan.Attributes;

public class RequiresPinAttribute : Attribute, IAsyncActionFilter // Note the change to IAsyncActionFilter
{
    private readonly P2PLoanDbContext dbContext;

    public RequiresPinAttribute(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;

        if (user == null || !user.Identity.IsAuthenticated)
        {
            // Return 401 Unauthorized if the user is not authenticated
            context.Result = new UnauthorizedObjectResult(new ServiceResponse<object>(ResponseStatus.Unauthorized, AppStatusCodes.Unauthorized, "Unauthorized", null));
            return;
        }

        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            // Return 401 Unauthorized if the user ID is invalid
            context.Result = new UnauthorizedObjectResult(new ServiceResponse<object>(ResponseStatus.Unauthorized, AppStatusCodes.Unauthorized, "Unauthorized", null));
            return;
        }

        var dbUser = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (dbUser == null || !dbUser.PinCreated)
        {
            // Return 400 Bad Request if the user has not created a PIN
            context.Result = new BadRequestObjectResult(new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.NoPinCreated, "No Pin Created", null));
            return;
        }

        // Proceed to the next action
        await next();
    }
}
