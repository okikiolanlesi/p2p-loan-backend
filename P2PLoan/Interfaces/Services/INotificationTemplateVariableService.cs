using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs.Requests;
using P2PLoan.Models;
using P2PLoan.Repositories;
using P2PLoan.Services;

namespace P2PLoan.Services;

public interface INotificationTemplateVariableService
{
    Task<NotificationTemplateVariable> CreateNotificationTemplateVariableAsync(NotificationTemplateVariableRequestDTO notificationTemplateVariableRequestDTO);
    Task<NotificationTemplateVariable> GetNotificationTemplateVariableByIdAsync(Guid id);
    Task GetAllNotificationTemplateVariableAsync();
    Task<NotificationTemplateVariableRequestDTO> UpdateNotificationTemplateVariableAsync(Guid id, NotificationTemplateVariableRequestDTO notificationTemplateVariableRequestDTO);
    Task DeleteNotificationTemplateVariableAsync();

}