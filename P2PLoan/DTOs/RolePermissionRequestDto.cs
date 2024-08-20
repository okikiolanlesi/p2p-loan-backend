using System;
using P2PLoan.Constants;

namespace P2PLoan.Models;

public class RolePermissionRequestDto
{
    public Guid roleId { get; set; }
    public Guid permissionId { get; set; }
}
