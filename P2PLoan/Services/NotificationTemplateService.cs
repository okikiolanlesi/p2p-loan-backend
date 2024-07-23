using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;
using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;

namespace P2PLoan.Services;

public class NotificationTemplateService : INotificationTemplateService
{
   private readonly INotificationTemplateRepository notificationTemplateRepository;

    public NotificationTemplateService(INotificationTemplateRepository notificationTemplateRepository)
    {
        this.notificationTemplateRepository = notificationTemplateRepository;
    }
    public async Task<NotificationTemplate> CreateNotificationAsync(NotificationTemplate notificationTemplate)
    {
        if (notificationTemplate == null)
    {
        throw new ArgumentNullException(nameof(notificationTemplate));
    }

        var createdTemplate = await notificationTemplateRepository.CreateAsync(notificationTemplate);
        return createdTemplate;
    }

    public async Task <IEnumerable<NotificationTemplate>>GetAllNotificationAsync(Guid id)
    {
        var GetTemplate = await notificationTemplateRepository.GetAllByIdAsync(id);
        if(GetTemplate == null)
        {
            throw new ArgumentException("");
        }
        return GetTemplate;
        
    }

    public async Task <NotificationTemplate>GetByIdAsync(Guid id)
    {
        var template = await notificationTemplateRepository.GetByIdAsync(id);
            if (template == null)
            {
                throw new KeyNotFoundException("NotificationTemplate not found.");
            }

            return template;
    }

    public async Task<NotificationTemplate>UpdateNotificationAsync(NotificationTemplate notificationTemplate,Guid id)
    {
         if (notificationTemplate == null)
    {
        throw new ArgumentNullException(nameof(notificationTemplate));
    }

    var updatedTemplate = await notificationTemplateRepository.UpdateAsync(notificationTemplate, id);
    if (updatedTemplate == null)
    {
        throw new KeyNotFoundException("NotificationTemplate not found.");
    }

    return updatedTemplate;

    }
}
