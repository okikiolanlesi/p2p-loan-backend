// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Org.BouncyCastle.Math.EC.Rfc7748;
// using P2PLoan.Data;
// using P2PLoan.Interfaces;
// using P2PLoan.Models;

// namespace P2PLoan.Repositories;

// public class NotificationTemplateRepository : INotificationTemplateRepository
// {
//      private readonly P2PLoanDbContext dbContext;
//     private readonly INotificationTemplateRepository notificationTemplateRepository;

//     public NotificationTemplateRepository(P2PLoanDbContext dbContext, INotificationTemplateRepository notificationTemplateRepository)
//     {
//         this.dbContext = dbContext;
//         this.notificationTemplateRepository = notificationTemplateRepository;
//     }
//     public Task CreateAsync(Notification notification, User user)
//     {
        
//     }

//     public Task<IEnumerable<Notification>> GetAllByUserIdAsync(Guid userId)
//     {
        
//     }

//     public Task<Notification> GetByIdAsync(Guid notoficationId, Guid userId)
//     {
    
//     }

//     public Task UpdateAsync(Notification notification, Guid userId)
//     {
        
//     }
// }
