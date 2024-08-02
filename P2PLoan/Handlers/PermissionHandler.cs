using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Requirements;

namespace P2PLoan.Handlers;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly P2PLoanDbContext _dbContext;

    public PermissionHandler(IHttpContextAccessor httpContextAccessor, P2PLoanDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        var user = _httpContextAccessor.HttpContext.User;

        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            context.Fail();
            return;
        }

        var userIdString = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            context.Fail();
            return;
        }

        var dbUser = await _dbContext.Users
            .Include(x => x.UserRoles)
            .ThenInclude(ur => ur.Role)
            .ThenInclude(r => r.Permissions)
            .ThenInclude(p => p.Module)
            .FirstOrDefaultAsync(x => x.Id == userId);

        if (dbUser == null)
        {
            context.Fail();
            return;
        }

        var userPermissions = dbUser.UserRoles
            .SelectMany(ur => ur.Role.Permissions)
            .ToList();

        var hasPermission = userPermissions.Any(p =>
            p.Module.Identifier == requirement.Module &&
            p.Action == requirement.Action) ||
           requirement.UserTypes.Any(x => x == dbUser.UserType);

        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
