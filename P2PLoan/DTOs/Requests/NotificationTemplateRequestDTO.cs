using System;

namespace P2PLoan.DTOs.Requests;

public class NotificationTemplateRequestDTO
{

    public string Title { get; set; } 
    public string message { get; set; }
     public string Name { get; set; }

        public string Description { get; set; }
        public string Content { get; set; }
}

