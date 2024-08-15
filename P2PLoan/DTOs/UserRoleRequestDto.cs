using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.DTOs
{
    public class UserRoleRequestDto
    {
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }

    public Guid CreatedById { get; set; }
    public Guid ModifiedById { get; set; }
    }
}