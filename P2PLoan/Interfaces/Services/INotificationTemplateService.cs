using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs.Requests;
using P2PLoan.Models;
using P2PLoan.Services;

namespace P2PLoan.Services;

public interface INotificationTemplateService
{
    Task<NotificationTemplate> CreateNotificationAsync(NotificationTemplateRequestDTO notificationTemplateRequestDTO);
    Task<NotificationTemplate> GetByIdAsync(Guid id);
    Task<IEnumerable<NotificationTemplate>> GetAllNotificationAsync(Guid id);
    Task<NotificationTemplate> UpdateNotificationAsync(NotificationTemplateRequestDTO notificationTemplateRequestDTO,Guid id);

}