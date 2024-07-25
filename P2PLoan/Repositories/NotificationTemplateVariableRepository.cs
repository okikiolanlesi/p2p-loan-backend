using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using P2PLoan.Data;
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

    public Task<NotificationTemplateVariable> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<NotificationTemplateVariable> UpdateAsync(NotificationTemplateVariable notificationTemplateVariable, Guid id)
    {
       if (notificationTemplateVariable == null)
            {
                throw new ArgumentNullException(nameof(notificationTemplateVariable));
            }
            var existingTemplate = await dbContext.NotificationTemplateVariables
                .Include(nt => nt.CreatedBy)
                .Include(nt => nt.ModifiedBy)
                .FirstOrDefaultAsync(nt => nt.Id == notificationTemplateVariable.Id);

            if (existingTemplate == null)
            {
                throw new KeyNotFoundException("NotificationTemplate not found.");
            }

            existingTemplate.Name = notificationTemplateVariable.Name;
            existingTemplate.Description = notificationTemplateVariable.Description;

            dbContext.NotificationTemplateVariables.Update(existingTemplate);
            await dbContext.SaveChangesAsync();
            return existingTemplate;
    }
}
