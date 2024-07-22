using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface INotificationTemplateRepository
{
    Task CreateAsync(NotificationTemplate notificationTemplate);
    Task<NotificationTemplate> GetByIdAsync(Guid NotificationTemplateId, Guid id);
    Task<IEnumerable<NotificationTemplate>> GetAllByIdAsync(Guid id);
    Task UpdateAsync(NotificationTemplate notificationTemplate, Guid id);

    Task Delete(NotificationTemplate notificationTemplate, Guid id);
}
