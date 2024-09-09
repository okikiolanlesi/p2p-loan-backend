using System;

namespace P2PLoan.DTOs
{
    public class UserRoleRequestDto
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}
