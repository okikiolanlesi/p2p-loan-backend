using Microsoft.AspNetCore.Mvc;
using P2PLoan.DTOs.Requests;
using P2PLoan.Models;
using P2PLoan.Services; // Assume this is where your service layer is located
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace P2PLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationTemplateController : ControllerBase
    {
        private readonly INotificationTemplateService _notificationTemplateService;

        public NotificationTemplateController(INotificationTemplateService notificationTemplateService)
        {
            _notificationTemplateService = notificationTemplateService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] NotificationTemplateRequestDTO notificationTemplateRequestDTO)
        {
                if (notificationTemplateRequestDTO == null)
            {
                return BadRequest("NotificationTemplateRequestDTO cannot be null.");
            }

            try
            {
                // Map DTO to Entity
                var notificationTemplate = new NotificationTemplate
                       {
                            Id = Guid.NewGuid(),
                            Title = notificationTemplateRequestDTO.Title,
                            Name = notificationTemplateRequestDTO.Name,
                            Description = notificationTemplateRequestDTO.Description,
                            Content = notificationTemplateRequestDTO.Content,
                            CreatedAt = DateTime.UtcNow,
                            ModifiedAt = DateTime.UtcNow,
                            CreatedById = notificationTemplateRequestDTO.CreatedById,
                            ModifiedById = notificationTemplateRequestDTO.ModifiedById
                        };

        // Create the template using the service
        var createdTemplate = await _notificationTemplateService.CreateNotificationAsync(notificationTemplate);

        // Return CreatedAtAction response
        return CreatedAtAction(nameof(GetById), new { id = createdTemplate.Id }, createdTemplate);
    }
    catch (Exception ex)
    {
        return StatusCode(500, $"Internal server error: {ex.Message}");
    }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var template = await _notificationTemplateService.GetByIdAsync(id);
                if (template == null)
                {
                    return NotFound();
                }

                return Ok(template);
            }
            catch (Exception ex)
            {
                // Log exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid id)
        {
            try
            {
                var templates = await _notificationTemplateService.GetAllNotificationAsync(id);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                // Log exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] NotificationTemplateRequestDTO notificationTemplateRequestDTO)
        {
            if (notificationTemplateRequestDTO == null)
            {
                return BadRequest("NotificationTemplate cannot be null.");
            }

          try
    {
        // Map DTO to Entity
        var notificationTemplate = new NotificationTemplate
        {
            Id = id,
            Title = notificationTemplateRequestDTO.Title,
            Name = notificationTemplateRequestDTO.Name,
            Description = notificationTemplateRequestDTO.Description,
            Content = notificationTemplateRequestDTO.Content,
            ModifiedAt = DateTime.UtcNow // Update the modified date to current time
        };

        // Update the template using the service
        var updatedTemplate = await _notificationTemplateService.UpdateNotificationAsync(notificationTemplate, id);

        if (updatedTemplate == null)
        {
            return NotFound();
        }

        return Ok(updatedTemplate);
    }
            catch (Exception ex)
            {
                // Log exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
