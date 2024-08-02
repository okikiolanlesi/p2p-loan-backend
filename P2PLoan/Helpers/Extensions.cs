using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using P2PLoan.Middlewares;

namespace P2PLoan.Helpers
{
    public static class Extensions
    {
        public static Guid GetLoggedInUserId(this ClaimsPrincipal claims)
        {
            var userIdString = claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(userIdString))
                throw new Exception("Session expired. Please logout and login again");

            var result = Guid.TryParse(userIdString, out var userId);

            if (!result)
            {
                throw new Exception("Invaid credentials. Please logout and login again");

            }
            return userId;
        }

        public static void UseCustomExceptionHandler(this IApplicationBuilder app) =>
           app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}