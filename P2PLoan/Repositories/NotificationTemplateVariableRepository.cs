using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using P2PLoan.Data;
using P2PLoan.DTOs.Requests;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Repositories;

public class NotificationTemplateVariableRepository : INotificationTemplateVariableRepository
{
    private readonly P2PLoanDbContext dbContext;

    public NotificationTemplateVariableRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
      
    }
    public async Task<NotificationTemplateVariable> CreateAsync(NotificationTemplateVariable notificationTemplateVariable)
    {
        if (notificationTemplateVariable == null)
    {
        throw new ArgumentNullException(nameof(notificationTemplateVariable));
    }

    // Optionally check if the related NotificationTemplate exists
    if (notificationTemplateVariable.NotificationTemplateId != Guid.Empty)
    {
        var relatedTemplate = await dbContext.NotificationTemplates.FindAsync(notificationTemplateVariable.NotificationTemplateId);
        if (relatedTemplate == null)
        {
            throw new ArgumentException("Related NotificationTemplate does not exist.");
        }
    }

    await dbContext.NotificationTemplateVariables.AddAsync(notificationTemplateVariable);
    await dbContext.SaveChangesAsync();
    return notificationTemplateVariable;
        
    }

    public Task<IEnumerable<NotificationTemplateVariable>> GetAllByUserIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<NotificationTemplateVariable> GetByIdAsync(Guid id)
    {
        return await dbContext.NotificationTemplateVariables
            .Include(v => v.NotificationTemplate) // Include related entities if needed
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<NotificationTemplateVariable> UpdateAsync(NotificationTemplateVariable notificationTemplateVariable, Guid id)
    {
       
           var existingTemplate = await dbContext.NotificationTemplateVariables
        .Include(nt => nt.CreatedBy)
        .Include(nt => nt.ModifiedBy)
        .Include(nt => nt.NotificationTemplate) // Assuming you need to include the related entity
        .FirstOrDefaultAsync(nt => nt.Id == id);

    if (existingTemplate == null)
    {
        throw new KeyNotFoundException("NotificationTemplateVariable not found.");
    }

    // Check if NotificationTemplateId is present
    if (notificationTemplateVariable.NotificationTemplateId == Guid.Empty)
    {
        throw new InvalidOperationException("NotificationTemplateId is required.");
    }

    // Update properties
    existingTemplate.Name = notificationTemplateVariable.Name;
    existingTemplate.Description = notificationTemplateVariable.Description;
    existingTemplate.ModifiedAt = notificationTemplateVariable.ModifiedAt;
    existingTemplate.CreatedAt = notificationTemplateVariable.CreatedAt;
    existingTemplate.NotificationTemplateId = notificationTemplateVariable.NotificationTemplateId;

    dbContext.NotificationTemplateVariables.Update(existingTemplate);
    await dbContext.SaveChangesAsync();

    return existingTemplate;

    }
}
