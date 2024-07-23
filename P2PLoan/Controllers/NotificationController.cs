// using System;
// using System.Collections.Generic;
// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using System.Text;
// using System.Threading.Tasks;
// using AutoMapper;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Configuration;
// using Microsoft.IdentityModel.Tokens;
// using P2PLoan.DTOs;
// using P2PLoan.DTOs.Requests;
// using P2PLoan.Interfaces;
// using P2PLoan.Models;
// using P2PLoan.Repositories;
// using P2PLoan.Services;

// namespace P2PLoan.Controllers;

// [ApiController]
// [Route("user/notification")]

// public class NotificationController : ControllerBase
// {
//     private readonly IUserRepository _userRepository;
//     private readonly IMapper _mapper;
//     private readonly IConfiguration _configuration;

//     private readonly INotificationService _notificationService;

//     public NotificationController(INotificationService notificationService,IUserRepository userRepository, IMapper mapper, IConfiguration configuration)
//     {
//         _userRepository = userRepository;
//         _mapper = mapper;
//         _configuration = configuration;
//         _notificationService = notificationService;


//     }

//     [HttpPost]
//     // public async Task<ActionResult>CreateNotification ([FromBody] NotificationRequestDTO notificationRequestDTO, Notification notification, Guid id)
//     // {
//     //      await _notificationService.CreateNotificationAsync(notification, id)


        

//     // }


// }
