using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface INotificationTemplateRepository
{
    Task CreateAsync(Notification notification, User user);
    Task<Notification> GetByIdAsync(Guid notoficationId, Guid userId);
    Task<IEnumerable<Notification>> GetAllByUserIdAsync(Guid userId);
    Task UpdateAsync(Notification notification, Guid userId);
}
