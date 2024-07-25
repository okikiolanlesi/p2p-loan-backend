using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface INotificationTemplateVariableRepository
{
    Task<NotificationTemplateVariable> CreateAsync(NotificationTemplateVariable notificationTemplateVariable);
    Task<NotificationTemplateVariable> GetByIdAsync(Guid id);
    Task<IEnumerable<NotificationTemplateVariable>> GetAllByUserIdAsync(Guid id);
    Task<NotificationTemplateVariable>UpdateAsync(NotificationTemplateVariable notificationTemplateVariable, Guid id);
}
