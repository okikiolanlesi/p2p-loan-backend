
using Microsoft.AspNetCore.Authorization;
using P2PLoan.Constants;
using P2PLoan.Models;

namespace P2PLoan.Attributes;

public class PermissionAttribute : AuthorizeAttribute
{
    public PermissionAttribute(Modules module, PermissionAction action, UserType userType)
    {
        Policy = $"Permission:{module}:{action}:{userType}";
    }
}