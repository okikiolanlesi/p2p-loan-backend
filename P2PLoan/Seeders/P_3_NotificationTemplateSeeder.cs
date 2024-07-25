using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders;

public class P_3_NotificationTemplateSeeder : ISeeder
{
      private readonly P2PLoanDbContext dbContext;
      private readonly INotificationTemplateRepository notificationTemplateRepository;

        public P_3_NotificationTemplateSeeder(INotificationTemplateRepository notificationTemplateRepository,P2PLoanDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.notificationTemplateRepository = notificationTemplateRepository;
        }

     public async Task up()
    {
         var userId = new Guid("ED7E3236-FEF6-405C-F63C-08DCACC0ACAD");
         // Check if the user exists
            var userExists = await dbContext.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new Exception($"User with ID {userId} does not exist. Please create this user first.");
            }

             // Ensure the database context is properly configured
            dbContext.Database.EnsureCreated();

            // Check if there are any existing NotificationTemplates
            if (await dbContext.NotificationTemplates.AnyAsync())
            {
                // Delete existing templates if needed
                var existingTemplates = await dbContext.NotificationTemplates.ToListAsync();
                dbContext.NotificationTemplates.RemoveRange(existingTemplates);
                await dbContext.SaveChangesAsync();
            }

            var templates = new List<NotificationTemplate>
            {
                new NotificationTemplate
                {
                    Id =Guid.NewGuid(),
                    Name = "Debit",
                    Description = "You have been debited by MS",
                    Title = "Title 1",
                    Content = "Content for Template 1",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedById = userId, // Replace with actual user ID
                    ModifiedById = userId
                },
                new NotificationTemplate
                {
                    Id =Guid.NewGuid(),
                    Name = "Credit Alert",
                    Description = "Your wallet has been credited",
                    Title = "Title 2",
                    Content = "Content for Template 2",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedById = userId, // Replace with actual user ID
                    ModifiedById = userId
                }
            };

          // Add new templates
            foreach (var template in templates)
            {
                await notificationTemplateRepository.CreateAsync(template);
            }
        }
        
    public string Description()
    {
        throw new NotImplementedException();
    }

    public Task down()
    {
        throw new NotImplementedException();
    }

   
}
