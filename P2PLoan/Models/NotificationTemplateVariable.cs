using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class NotificationTemplateVariable: AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        // foreign key
        public Guid NotificationTemplateId { get; set; }
        public NotificationTemplate NotificationTemplate{ get; set; }
        
    }
}