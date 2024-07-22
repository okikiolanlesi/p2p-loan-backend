using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;
using AutoMapper;

namespace P2PLoan.Services;

public class NotificationTemplateVariableService : INotificationTemplateVariableService
{
    private readonly INotificationTemplateVariableRepository notificationTemplateVariableRepository;
    private readonly IMapper mapper;
    private readonly IConstants constant;

    public NotificationTemplateVariableService( IMapper mapper, INotificationTemplateVariableRepository notificationTemplateRepository)
    {
        this.notificationTemplateVariableRepository= notificationTemplateVariableRepository;
        this.mapper = mapper;

    }
    public Task CreateNotificationTemplateVariableAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task DeleteNotificationTemplateVariableAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task GetAllNotificationTemplateVariableAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task GetNotificationTemplateVariableByIdAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task UpdateNotificationTemplateVariableAsync()
    {
        throw new System.NotImplementedException();
    }
}
