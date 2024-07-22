using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;
using AutoMapper;

namespace P2PLoan.Services;

public class NotificationTemplateService : INotificationTemplateService
{
    private readonly INotificationTemplateRepository notificationTemplateRepository;
    private readonly IMapper mapper;
    private readonly IConstants constant;

    public NotificationTemplateService( IMapper mapper, INotificationRepository notificationRepository)
    {
        this.notificationTemplateRepository= notificationTemplateRepository;
        this.mapper = mapper;

    }
    public Task CreateNotificationAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task GetAllNotificationAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task GetByIdAsync()
    {
        throw new System.NotImplementedException();
    }

    public Task UpdateNotificationAsync()
    {
        throw new System.NotImplementedException();
    }
}
