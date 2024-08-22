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

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserProfile(Guid id)
        {
            var response = await userService.GetUserById(id);
               return ControllerHelper.HandleApiResponse(response);
        }
        
    }
}