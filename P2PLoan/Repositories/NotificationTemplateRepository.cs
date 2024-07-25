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

public class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly P2PLoanDbContext dbContext;
    

    public NotificationTemplateRepository(P2PLoanDbContext dbContext)
    {
        this.dbContext = dbContext;
      
    }

    public async Task<NotificationTemplate> CreateAsync(NotificationTemplate notificationTemplate)
    {
        if (notificationTemplate == null)
            {
                throw new ArgumentNullException(nameof(notificationTemplate));
            }

                // Assign valid user IDs for CreatedBy and ModifiedBy
            if (notificationTemplate.CreatedById == Guid.Empty)
            {
                throw new ArgumentException("CreatedById cannot be empty.", nameof(notificationTemplate.CreatedById));
            }

            if (notificationTemplate.ModifiedById == Guid.Empty)
            {
                throw new ArgumentException("ModifiedById cannot be empty.", nameof(notificationTemplate.ModifiedById));
            }

            await dbContext.NotificationTemplates.AddAsync(notificationTemplate);
            await dbContext.SaveChangesAsync();
            return notificationTemplate;
       
    }

    public async Task<IEnumerable<NotificationTemplate>> GetAllByIdAsync(Guid id)
    {
        return await dbContext.NotificationTemplates.ToListAsync();
    }

    public async Task<NotificationTemplate> GetByIdAsync( Guid id)
    {
        return await dbContext.NotificationTemplates
                .FirstOrDefaultAsync(n => n.Id == id);
       
    }

    public async Task<NotificationTemplate> UpdateAsync(NotificationTemplate notificationTemplate, Guid id)
    {
        if (notificationTemplate == null)
            {
                throw new ArgumentNullException(nameof(notificationTemplate));
            }

            var existingTemplate = await dbContext.NotificationTemplates
                .Include(nt => nt.NotificationTemplateVariables)
                .Include(nt => nt.CreatedBy)
                .Include(nt => nt.ModifiedBy)
                .FirstOrDefaultAsync(nt => nt.Id == notificationTemplate.Id);

            if (existingTemplate == null)
            {
                throw new KeyNotFoundException("NotificationTemplate not found.");
            }

            existingTemplate.Name = notificationTemplate.Name;
            existingTemplate.Description = notificationTemplate.Description;
            existingTemplate.Title = notificationTemplate.Title;
            existingTemplate.Content = notificationTemplate.Content;
            existingTemplate.ModifiedAt = DateTime.UtcNow;

            dbContext.NotificationTemplates.Update(existingTemplate);
            await dbContext.SaveChangesAsync();
            return existingTemplate;
    }

}
