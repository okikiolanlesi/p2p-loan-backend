using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;
using AutoMapper;
using System;
using Microsoft.EntityFrameworkCore;
using P2PLoan.DTOs.Requests;
using Microsoft.AspNetCore.Http.HttpResults;

namespace P2PLoan.Services;

public class NotificationTemplateService : INotificationTemplateService
{
   private readonly INotificationTemplateRepository notificationTemplateRepository;

    public NotificationTemplateService(INotificationTemplateRepository notificationTemplateRepository)
    {
        this.notificationTemplateRepository = notificationTemplateRepository;
    }
    public async Task<NotificationTemplate> CreateNotificationAsync(NotificationTemplateRequestDTO notificationTemplateRequestDTO)
    {
        // Map DTO to Entity
       var notificationTemplate = new NotificationTemplate
        {
            Title = notificationTemplateRequestDTO.Title,
            Name = notificationTemplateRequestDTO.Name,
            Description = notificationTemplateRequestDTO.Description,
            Content = notificationTemplateRequestDTO.Content,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            CreatedById = notificationTemplateRequestDTO.CreatedById,
            ModifiedById = notificationTemplateRequestDTO.ModifiedById

        };

        var createdTemplate = await notificationTemplateRepository.CreateAsync(notificationTemplate);
        if(createdTemplate is null) 
        throw new ArgumentException("fhfh");

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

    public async Task<NotificationTemplate>UpdateNotificationAsync(NotificationTemplateRequestDTO notificationTemplateRequestDTO,Guid id)
    {
        // check for valid id
        var notificationTemplateId = await notificationTemplateRepository.GetByIdAsync(id);
        if(notificationTemplateId is null)
        throw new KeyNotFoundException("NotificationTemplateID not found.");
         // Map DTO to Entity
        var notificationTemplate = new NotificationTemplate
        {
            Id = id,
            Title = notificationTemplateRequestDTO.Title,
            Name = notificationTemplateRequestDTO.Name,
            Description = notificationTemplateRequestDTO.Description,
            Content = notificationTemplateRequestDTO.Content,
            ModifiedAt = DateTime.UtcNow,
            CreatedById = notificationTemplateRequestDTO.CreatedById,
            ModifiedById = notificationTemplateRequestDTO.ModifiedById // Update the modified date to current time
        };

    var updatedTemplate = await notificationTemplateRepository.UpdateAsync(notificationTemplate, id);
    if (updatedTemplate is null)    
    throw new KeyNotFoundException("NotificationTemplate not found.");
    
    return updatedTemplate;

    }
}
