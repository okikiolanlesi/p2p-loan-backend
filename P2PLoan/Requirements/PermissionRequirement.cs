using Microsoft.AspNetCore.Authorization;
using P2PLoan;
using P2PLoan.Constants;
using P2PLoan.Models;

namespace P2PLoan.Requirements;
public class PermissionRequirement : IAuthorizationRequirement
{
    public Modules Module { get; }
    public PermissionAction Action { get; }
    public UserType UserType { get; }

    public PermissionRequirement(Modules module, PermissionAction action, UserType userType)
    {
        Module = module;
        Action = action;
        UserType = userType;
    }
}