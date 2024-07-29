using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface INotificationTemplateRepository
{
    Task <NotificationTemplate>CreateAsync(NotificationTemplate notificationTemplate);
    Task<NotificationTemplate?> GetByIdAsync(Guid id);
    Task<IEnumerable<NotificationTemplate>> GetAllByIdAsync(Guid id);
    Task<NotificationTemplate> UpdateAsync(NotificationTemplate notificationTemplate, Guid id);

}
