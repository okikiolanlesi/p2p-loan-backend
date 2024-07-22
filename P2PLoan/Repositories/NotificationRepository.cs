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

// public class NotificationRepository : INotificationRepository
// {
//     private readonly P2PLoanDbContext dbContext;
//     private readonly IUserRepository userRepository;

//     public NotificationRepository(P2PLoanDbContext dbContext, IUserRepository userRepository)
//     {
//         this.dbContext = dbContext;
//         this.userRepository = userRepository;
//     }
//     public async Task CreateAsync(Notification notification, Guid id)
//     {
//         var userExits = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
//         if(userExits == null)
//         {
//             throw new NotImplementedException();
//         }
//         dbContext.Notifications.Add(notification);

//         await dbContext.SaveChangesAsync();

        
//     }

//     public async Task<IEnumerable<Notification>> GetAllByUserIdAsync(Guid notificationId, Guid id)
//     {
//          var userExits = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
//         if(userExits == null)
//         {
//             throw new NotImplementedException();
//         }

//         return await dbContext.Notifications
//              .ToListAsync();
//     }

//     public Task<Notification> GetByIdAsync(Guid notoficationId, Guid userId)
//     {
//         var notification = await dbContext.Notifications
//         .Include(n => n.User)
//         .FirstOrDefaultAsync(n => n.NotificationId == notificationId);

//         if(notification == null)
//         {
//             throw new NotImplementedException();

//         }

//         return notification;
//     }

//     public async Task UpdateAsync(Guid notificationId, Guid id, Notification updateNotification)
//     {
//         //check if the notification exits and belong to the users
//         var existingNotification = await dbContext.Notifications
//             .FirstOrDefaultAsync(n =>n.Id == notificationId && n.UserId ==id);
//             if(existingNotification == null)
//             {
//                 throw new NotImplementedException();

//             }
//             // update
        
//     }

// }
