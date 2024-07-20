using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Services;
using AutoMapper;


namespace P2PLoan.Services;

public class NotificationService : INotificationService
{
    private readonly IUserRepository userRepository;
    private readonly INotificationRepository notificationRepository;
    private readonly IMapper mapper;
    private readonly IEmailService emailService;
    private readonly IConstants constant;

    public NotificationService(IUserRepository userRepository, IMapper mapper, INotificationRepository notificationRepository, IEmailService emailService, IConstants constants)
    {
        this.userRepository = userRepository;
        this.notificationRepository= notificationRepository;
        this.mapper = mapper;
        this.emailService = emailService;
        this.constant = constants;
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
