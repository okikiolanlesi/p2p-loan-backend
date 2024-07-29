using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Seeders
{
    public class P_4_NotificationTemplateVariableSeeder : ISeeder
    {
        private readonly P2PLoanDbContext dbContext;
        private readonly INotificationTemplateRepository notificationTemplateRepository;
        private readonly INotificationTemplateVariableRepository notificationTemplateVariableRepository;

        public P_4_NotificationTemplateVariableSeeder(
            INotificationTemplateRepository notificationTemplateRepository,
            INotificationTemplateVariableRepository notificationTemplateVariableRepository,
            P2PLoanDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.notificationTemplateRepository = notificationTemplateRepository;
            this.notificationTemplateVariableRepository = notificationTemplateVariableRepository;
        }
        public async Task up()
        {
            var id = new Guid("ED2F06AE-5BEC-4FF0-8357-3F3B6F2C7D5F");
            
         var userId = new Guid("ED7E3236-FEF6-405C-F63C-08DCACC0ACAD");
         // Check if the user exists
            var userExists = await dbContext.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                throw new Exception($"User with ID {userId} does not exist. Please create this user first.");
            }
             // Ensure the database context is properly configured
            dbContext.Database.EnsureCreated();

            // Check if there are any existing NotificationTemplateVariables
            if (await dbContext.NotificationTemplateVariables.AnyAsync())
            {
                // Delete existing variables if needed
                var existingVariables = await dbContext.NotificationTemplateVariables.ToListAsync();
                dbContext.NotificationTemplateVariables.RemoveRange(existingVariables);
                await dbContext.SaveChangesAsync();
            }

            // Ensure related NotificationTemplates exist
            var templates = await dbContext.NotificationTemplates.ToListAsync();
            if (!templates.Any())
            {
                throw new Exception("No NotificationTemplates found. Please seed NotificationTemplates first.");
            }

            var variables = new List<NotificationTemplateVariable>
            {
                new NotificationTemplateVariable
                {
                    Id = Guid.NewGuid(),
                    Name = "LoanApproval",
                    Description = "Variable for loan approval message",
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    NotificationTemplateId = id ,
                    CreatedById = userId, // Replace with actual user ID
                    ModifiedById = userId// Use a valid NotificationTemplateId
                }
                // new NotificationTemplateVariable
                // {
                //     Id = Guid.NewGuid(),
                //     Name = "AccountUpdate",
                //     Description = "Variable for account update message",
                //     CreatedAt = DateTime.UtcNow,
                //     ModifiedAt = DateTime.UtcNow,
                //     NotificationTemplateId = id ,
                //     CreatedById = userId, // Replace with actual user ID
                //     ModifiedById = userId// Use a valid NotificationTemplateId a valid NotificationTemplateId
                // }
            };

            // Add new variables
            foreach (var variable in variables)
            {
                await notificationTemplateVariableRepository.CreateAsync(variable);
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