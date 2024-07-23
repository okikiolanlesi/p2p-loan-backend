using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;
using AutoMapper;
using P2PLoan.DTOs.Requests;
using System;


namespace P2PLoan.Services;

public class NotificationService : INotificationService
{
    private readonly IUserRepository userRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper mapper;
    private readonly IEmailService emailService;
    private readonly IConstants constant;

    public NotificationService(IUserRepository userRepository, IMapper mapper, INotificationRepository InotificationRepository, IEmailService emailService, IConstants constants)
    {
        this.userRepository = userRepository;
        this._notificationRepository= _notificationRepository;
       // this.mapper = mapper;
       // this.emailService = emailService;
       // this.constant = constants;
    }

    public async Task CreateNotificationAsync(Notification notification, Guid id)
    {
      await _notificationRepository.CreateAsync(notification, id);      
    }
    public Task GetAllNotificationAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task GetByIdAsync(Guid NotificationId, Guid userId)
    {
        throw new NotImplementedException();
    }

   public async Task UpdateNotificationAsync(Guid notificationId, Guid userId, Notification notification)
    {
        var updatedNotification = await _notificationRepository.UpdateAsync(notificationId, userId, notification);
        if (updatedNotification == null)
        {
            throw new KeyNotFoundException("Notification not found");
        }
    }
}
