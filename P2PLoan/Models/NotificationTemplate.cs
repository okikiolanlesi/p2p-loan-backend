using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace P2PLoan.Models
{
    public class NotificationTemplate:AuditableEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        //nagivation properties

        public ICollection<NotificationTemplateVariable> NotificationTemplateVariables{ get; set; }
    }
}