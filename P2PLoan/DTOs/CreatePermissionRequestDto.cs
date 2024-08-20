using System;
using P2PLoan.Constants;

namespace P2PLoan.Models;

public class CreatePermissionRequestDto
{
    public Guid ModuleId { get; set; }
    public PermissionAction Action { get; set; }

    public Guid CreatedById { get; set; }
    public Guid ModifiedById { get; set; }
}
