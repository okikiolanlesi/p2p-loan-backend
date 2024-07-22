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
    public Task CreateAsync(Notification notification, User user)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Notification>> GetAllByUserIdAsync(Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Notification> GetByIdAsync(Guid notoficationId, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Notification notification, Guid userId)
    {
        throw new NotImplementedException();
    }
}

public interface INotificationTemplateVariable
{
}