using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.DTOs.Requests
{
    public class NotificationTemplateVariableRequestDTO
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public Guid CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
        public Guid NotificationTemplateId { get; set; }
    }
}