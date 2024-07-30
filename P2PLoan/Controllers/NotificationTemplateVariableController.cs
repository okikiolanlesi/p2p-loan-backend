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
            var createTemplateVariable = await notificationTemplateVariableService.CreateNotificationTemplateVariableAsync(notificationTemplateVariableRequestDTO);
            if (createTemplateVariable == null)  return NotFound();
            return Ok(createTemplateVariable);         

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
