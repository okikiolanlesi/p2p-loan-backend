using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.DTOs.Requests;
using P2PLoan.Models;
using P2PLoan.Services;

namespace P2PLoan.Services;

public interface INotificationService
{
    Task CreateNotificationAsync(Notification notification, Guid id);
    Task GetByIdAsync(Guid NotificationId, Guid userId);
    Task GetAllNotificationAsync(Guid userId);
    Task UpdateNotificationAsync(Guid notificationId,Guid id, Notification notification);

}