using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Models;

namespace P2PLoan.Interfaces;

public interface INotificationRepository
{
    Task CreateAsync(Notification notification, Guid id);
    Task<Notification> GetByIdAsync(Guid notificationId);
    Task<IEnumerable<Notification>> GetAllByUserIdAsync(Guid notificationId, Guid id);
    Task<Notification> UpdateAsync(Guid notificationId, Guid id, Notification updateNotification);
    
    
}
