using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;

namespace P2PLoan.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IMapper mapper;
    public UserService(IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.httpContextAccessor = httpContextAccessor;
        this.mapper = mapper;

    }

    public async Task<ServiceResponse<object>> GetCurrentUserProfile()
    {
        var loggedInUserId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();
        if (loggedInUserId == null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var user = await userRepository.GetByIdAsync(loggedInUserId);
        if (user == null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var userDto = mapper.Map<UserDto>(user);
        return new ServiceResponse<object>(
       ResponseStatus.Success,
       AppStatusCodes.Success,
       "User retrieved successfully",
       userDto
   );
    }


    public async Task<ServiceResponse<object>> GetPublicUserProfileById(Guid userId)
    {
        var loggedInUserId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();
        if (loggedInUserId == null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User not found", null);

        }

        var publicUserDto = mapper.Map<PublicUserProfileDto>(user);
        return new ServiceResponse<object>(
        ResponseStatus.Success,
        AppStatusCodes.Success,
        "User retrieved successfully",
        publicUserDto
        );

    }
}
