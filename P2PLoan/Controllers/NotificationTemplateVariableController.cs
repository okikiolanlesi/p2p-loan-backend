using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P2PLoan.DTOs.Requests;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;

namespace P2PLoan.Controllers
{
   [ApiController]
   [Route("api/[controller]")]
    public class NotificationTemplateVariableController : ControllerBase
    {
            private readonly INotificationTemplateVariableService notificationTemplateVariableService;
            private readonly INotificationTemplateRepository notificationTemplateRepository;

            public NotificationTemplateVariableController(INotificationTemplateVariableService notificationTemplateVariableService, INotificationTemplateRepository notificationTemplateRepository)
            {
                this.notificationTemplateVariableService = notificationTemplateVariableService ?? throw new ArgumentNullException(nameof(notificationTemplateVariableService));
                this.notificationTemplateRepository = notificationTemplateRepository;
            }

        [HttpPost]
        public async Task<ActionResult<NotificationTemplateVariable>> Create([FromBody] NotificationTemplateVariableRequestDTO notificationTemplateVariableRequestDTO)
        {
            if (notificationTemplateVariableRequestDTO == null)
            {
                return BadRequest("NotificationTemplateRequestDTO cannot be null.");
            }
            var notificationTemplate = await notificationTemplateRepository.GetByIdAsync(notificationTemplateVariableRequestDTO.NotificationTemplateId);
            if (notificationTemplate is null)
            {
                return BadRequest("Notification Template does not exist");
            }

            try
            {
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

                // Log mapped entity properties
                Console.WriteLine($"Mapped Entity: Id = {notificationTemplateVariable.Id}, Name = {notificationTemplateVariable.Name}, Description = {notificationTemplateVariable.Description}, CreatedAt = {notificationTemplateVariable.CreatedAt}, ModifiedAt = {notificationTemplateVariable.ModifiedAt},CreatedById={notificationTemplateVariable.CreatedById}");

                // Create the template using the service
                var createdTemplate = await notificationTemplateVariableService.CreateNotificationTemplateVariableAsync(notificationTemplateVariable);

                // Log created entity properties
                Console.WriteLine($"Created Entity: Id = {createdTemplate?.Id}, Name = {createdTemplate?.Name}, Description = {createdTemplate?.Description}, CreatedAt = {createdTemplate?.CreatedAt}, ModifiedAt = {createdTemplate?.ModifiedAt}");

                // Return the created entity
                return  createdTemplate;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
            public async Task<ActionResult<NotificationTemplateVariable>> GetVariableById([FromRoute] Guid id)
            {
                  
                    var templateVariable = await notificationTemplateVariableService.GetNotificationTemplateVariableByIdAsync(id);
                    if (templateVariable == null)   return NotFound();
                
                    return Ok(templateVariable); 
        
            }
         [HttpPatch]
        [Route("{id:Guid}")]

        public async Task<ActionResult<NotificationTemplateVariableRequestDTO>> UpdateVariable([FromRoute] Guid id, [FromBody] NotificationTemplateVariableRequestDTO notificationTemplateVariableRequestDTO)
           {
            var updateTemplateVariable = await notificationTemplateVariableService.UpdateNotificationTemplateVariableAsync(id, notificationTemplateVariableRequestDTO);
            if (updateTemplateVariable== null) return NotFound();
            return updateTemplateVariable;       
           }
           
    }   
    
}
