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
        public async Task<ActionResult<NotificationTemplate>> Create([FromBody] NotificationTemplateRequestDTO notificationTemplateRequestDTO)
        {
            var notificationTemplate = await _notificationTemplateService.CreateNotificationAsync(notificationTemplateRequestDTO);
            if (notificationTemplate is null) 
            return BadRequest("Failed to create notification template.");
            return StatusCode (201, notificationTemplate);     
  
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
                     
                var template = await _notificationTemplateService.GetByIdAsync(id);
                if (template is null)
                    return NotFound();
                return Ok(template);   
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
        public async Task<ActionResult<NotificationTemplateRequestDTO>> Update(Guid id, [FromBody] NotificationTemplateRequestDTO notificationTemplateRequestDTO)
        {
            try
            {
                var updateNotification = await _notificationTemplateService.UpdateNotificationAsync(notificationTemplateRequestDTO, id);
                return StatusCode(201, updateNotification);


            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception (not shown)
                return StatusCode(500, "An unexpected error occurred.");
            }


        }
    }
}
