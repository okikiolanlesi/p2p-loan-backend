using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;
using P2PLoan.Repositories;
using P2PLoan.Services;

namespace P2PLoan.Services;

public interface INotificationTemplateVariableService
{
    Task<NotificationTemplateVariable> CreateNotificationTemplateVariableAsync(NotificationTemplateVariable notificationTemplateVariable);
    Task<NotificationTemplateVariable> GetNotificationTemplateVariableByIdAsync();
    Task GetAllNotificationTemplateVariableAsync();
    Task<NotificationTemplateVariable> UpdateNotificationTemplateVariableAsync(Guid id, NotificationTemplateVariable notificationTemplateVariable);
    Task DeleteNotificationTemplateVariableAsync();

}