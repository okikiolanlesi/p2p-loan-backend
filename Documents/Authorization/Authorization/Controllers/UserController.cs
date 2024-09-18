using Authorization.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Authorization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("Admins")]
        [Authorize(Roles = "Admin")]
        public IActionResult SupervisorsEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi, welcome {currentUser.FirstName}, you are an {currentUser.Role}");
        }


        [HttpGet("Employees")]
        [Authorize(Roles = "Employee")]
        public IActionResult DesignersEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi, welcome {currentUser.FirstName}, you are a {currentUser.Role}");
        }

        [HttpGet("SupervisorsAndDesignersAndDevelopers")]
        [Authorize(Roles = "Supervisor,Designer,Developer")]
        public IActionResult SupervisorsAndDesignersAndDevelopersEndpoint()
        {
            var currentUser = GetCurrentUser();

            return Ok($"Hi, welcome {currentUser.FirstName}, you are an {currentUser.Role}");
        }

        [HttpGet("Public")]
        public IActionResult Public()
        {
            return Ok("Hi, you are on TracksifyApp");
        }
        private UserModel GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new UserModel
                {
                    FirstName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value,
                    LastName = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.GivenName)?.Value,
                    Email = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Email)?.Value,
                    Role = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value
                };
            }
            return null;
        }
        }
    }

