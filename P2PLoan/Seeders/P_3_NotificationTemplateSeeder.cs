using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders
{
    public class P_3_NotificationTemplateSeeder : ISeeder
    {
        private readonly P2PLoanDbContext dbContext;
        private readonly INotificationTemplateRepository notificationTemplateRepository;

        public P_3_NotificationTemplateSeeder(INotificationTemplateRepository notificationTemplateRepository, P2PLoanDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.notificationTemplateRepository = notificationTemplateRepository;
        }

        public async Task up()
        {
            var id = new Guid("ED2F06AE-5BEC-4FF0-8357-3F3B6F2C7D5F");
            var id2 = new Guid("9D98E6DD-2F83-48C9-8106-38C1E19CF0BD");

            var userId = new Guid("ED7E3236-FEF6-405C-F63C-08DCACC0ACAD");

            // Check if the user exists
            var userExists = await dbContext.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new Exception($"User with ID {userId} does not exist. Please create this user first.");
            }

            // Ensure the database context is properly configured
            await dbContext.Database.EnsureCreatedAsync();

            // Check if the NotificationTemplates with specific IDs already exist
            var existingTemplate1 = await dbContext.NotificationTemplates.FindAsync(id);
            var existingTemplate2 = await dbContext.NotificationTemplates.FindAsync(id2);

            var templates = new List<NotificationTemplate>();

            if (existingTemplate1 == null)
            {
                templates.Add(new NotificationTemplate
                {
                    Id = id,
                    Name = "Debit",
                    Description = "You have been debited by MS",
                    Title = "Title 1",
                    Content = "Content for Template 1",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedById = userId,
                    ModifiedById = userId
                });
            }

            if (existingTemplate2 == null)
            {
                templates.Add(new NotificationTemplate
                {
                    Id = id2,
                    Name = "Credit Alert",
                    Description = "Your wallet has been credited",
                    Title = "Title 2",
                    Content = "Content for Template 2",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    CreatedById = userId,
                    ModifiedById = userId
                });
            }

            // Add new templates
            if (templates.Count > 0)
            {
                foreach (var template in templates)
                {
                    await notificationTemplateRepository.CreateAsync(template);
                }
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
}
