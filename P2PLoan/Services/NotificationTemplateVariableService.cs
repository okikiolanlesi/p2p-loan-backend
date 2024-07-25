using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;
using AutoMapper;
using P2PLoan.Repositories;
using System;

namespace P2PLoan.Services;

public class NotificationTemplateVariableService : INotificationTemplateVariableService
{
    private readonly INotificationTemplateVariableRepository notificationTemplateVariableRepository;
  

    public NotificationTemplateVariableService( INotificationTemplateVariableRepository notificationTemplateRepository)
    {
        this.notificationTemplateVariableRepository= notificationTemplateVariableRepository;

    }
    public async Task<NotificationTemplateVariable> CreateNotificationTemplateVariableAsync(NotificationTemplateVariable notificationTemplateVariable)
    {
         if (notificationTemplateVariable == null)
    {
        throw new ArgumentNullException(nameof(notificationTemplateVariable));
    }

        var createdTemplate = await notificationTemplateVariableRepository.CreateAsync(notificationTemplateVariable);
        return createdTemplate;
        
    }

    public Task DeleteNotificationTemplateVariableAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task GetAllNotificationTemplateVariableAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task<NotificationTemplateVariable> GetNotificationTemplateVariableByIdAsync()
    {
        throw new System.NotImplementedException();
    }

    public async Task<NotificationTemplateVariable> UpdateNotificationTemplateVariableAsync(Guid id,NotificationTemplateVariable notificationTemplateVariable)
    {
           if (notificationTemplateVariable == null)
    {
        throw new ArgumentNullException(nameof(notificationTemplateVariable));
    }

    var updatedTemplate = await notificationTemplateVariableRepository.UpdateAsync(notificationTemplateVariable, id);
    if (updatedTemplate == null)
    {
        throw new KeyNotFoundException("NotificationTemplate not found.");
    }

    return updatedTemplate;

    }
}

