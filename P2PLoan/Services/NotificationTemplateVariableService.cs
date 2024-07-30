using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;
using AutoMapper;
using P2PLoan.Repositories;
using System;
using P2PLoan.DTOs.Requests;

namespace P2PLoan.Services;

public class NotificationTemplateVariableService : INotificationTemplateVariableService
{
     private readonly INotificationTemplateVariableRepository notificationTemplateVariableRepository;
     private readonly INotificationTemplateRepository notificationTemplateRepository;

  

    public NotificationTemplateVariableService( INotificationTemplateVariableRepository notificationTemplateVariableRepository,INotificationTemplateRepository notificationTemplateRepository)
    {
        this.notificationTemplateVariableRepository= notificationTemplateVariableRepository;
         this.notificationTemplateRepository = notificationTemplateRepository;

    }
    public async Task<NotificationTemplateVariable> CreateNotificationTemplateVariableAsync(NotificationTemplateVariableRequestDTO notificationTemplateVariableRequestDTO)
    {
         if (notificationTemplateVariableRequestDTO == null)
            {
                throw new ArgumentNullException(nameof(notificationTemplateVariableRequestDTO));
            }
            
       var notificationTemplate = await notificationTemplateRepository.GetByIdAsync(notificationTemplateVariableRequestDTO.NotificationTemplateId);
          
            if (notificationTemplate is null)
            {
                throw new ArgumentException("fhfh");
            }

                // Log incoming request DTO
                Console.WriteLine($"Received DTO: Name = {notificationTemplateVariableRequestDTO.Name}, Description = {notificationTemplateVariableRequestDTO.Description}");

                // Map DTO to Entity
                var notificationTemplateVariable = new NotificationTemplateVariable
                {
                    Name = notificationTemplateVariableRequestDTO.Name,
                    Description = notificationTemplateVariableRequestDTO.Description,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedById = notificationTemplateVariableRequestDTO.CreatedById,
                    ModifiedById = notificationTemplateVariableRequestDTO.ModifiedById,
                    NotificationTemplate = notificationTemplate
                };
            
                // Create the template using the service
                var createdTemplate = await notificationTemplateVariableRepository.CreateAsync(notificationTemplateVariable);

                // Log created entity properties
                Console.WriteLine($"Created Entity: Id = {createdTemplate?.Id}, Name = {createdTemplate?.Name}, Description = {createdTemplate?.Description}, CreatedAt = {createdTemplate?.CreatedAt}, ModifiedAt = {createdTemplate?.ModifiedAt}");

                // Return the created entity
                return  createdTemplate;
        
    }

   

    public Task DeleteNotificationTemplateVariableAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task GetAllNotificationTemplateVariableAsync()
    {
        throw new System.NotImplementedException();
    }

    public async Task<NotificationTemplateVariable> GetNotificationTemplateVariableByIdAsync(Guid id)
    {
        
      var notificationTemplateVariable = await notificationTemplateVariableRepository.GetByIdAsync(id);
      if(notificationTemplateVariable == null)
      {
        
        throw new ArgumentNullException(nameof(notificationTemplateVariable));
      }
      return notificationTemplateVariable;
        
    }

    public async Task<NotificationTemplateVariableRequestDTO> UpdateNotificationTemplateVariableAsync(Guid id,NotificationTemplateVariableRequestDTO notificationTemplateVariableRequestDTO)
    {
        // Retrieve the existing entity from the repository    
    var updatedTemplate = await notificationTemplateVariableRepository.GetByIdAsync(id);

    if (updatedTemplate == null)
    {
        throw new KeyNotFoundException("NotificationTemplateVariable not found.");
    }
      
       updatedTemplate.Name = notificationTemplateVariableRequestDTO.Name;
       updatedTemplate.Description = notificationTemplateVariableRequestDTO.Description;
       updatedTemplate.ModifiedById=notificationTemplateVariableRequestDTO.ModifiedById;
       updatedTemplate.CreatedById=notificationTemplateVariableRequestDTO.CreatedById;
       updatedTemplate.NotificationTemplateId = notificationTemplateVariableRequestDTO.NotificationTemplateId;

    // Update the entity in the repository
    await notificationTemplateVariableRepository.UpdateAsync(updatedTemplate, id);
       // Map the updated entity back to DTO
    var updatedDTO = new NotificationTemplateVariableRequestDTO
    {
        Name = updatedTemplate.Name,
        Description = updatedTemplate.Description,
        CreatedById = updatedTemplate.CreatedById, // Include other properties as needed
        ModifiedById = updatedTemplate.ModifiedById,
        NotificationTemplateId = updatedTemplate.NotificationTemplateId,
    };

    return updatedDTO;

    }
}

