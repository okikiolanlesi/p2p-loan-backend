using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;


namespace P2PLoan.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetMyProfile()
        {
            var response = await userService.GetCurrentUserProfile();
            return ControllerHelper.HandleApiResponse(response);
        }

        [HttpGet("{id:guid}")]
        [Authorize]
        public async Task<IActionResult> GetPublicUserProfile(Guid id)
        {
            var response = await userService.GetPublicUserProfileById(id);
            return ControllerHelper.HandleApiResponse(response);

        }
    }
}