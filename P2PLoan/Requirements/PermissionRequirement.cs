using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using P2PLoan;
using P2PLoan.Constants;
using P2PLoan.Models;

namespace P2PLoan.Requirements;
public class PermissionRequirement : IAuthorizationRequirement
{
    public Modules Module { get; }
    public PermissionAction Action { get; }
    public IEnumerable<UserType> UserTypes { get; }

    public PermissionRequirement(Modules module, PermissionAction action, IEnumerable<UserType> userTypes)
    {
        Module = module;
        Action = action;
        UserTypes = userTypes;
    }
}