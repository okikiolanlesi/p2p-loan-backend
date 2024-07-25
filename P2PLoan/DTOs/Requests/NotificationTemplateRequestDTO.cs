using System;

namespace P2PLoan.DTOs.Requests;

public class NotificationTemplateRequestDTO
{

  public string Name { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid CreatedById { get; set; }
    public Guid ModifiedById { get; set; }
}

