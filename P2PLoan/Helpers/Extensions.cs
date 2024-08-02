using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using P2PLoan.Middlewares;

namespace P2PLoan.Helpers
{
    public static class Extensions
    {
        public static string GetLoggedInUserId(this ClaimsPrincipal claims)
        {
            var userId = claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            if (string.IsNullOrEmpty(userId))
                throw new Exception("Session expired. Please logout and login again");
            return userId;
        }

        public static void UseCustomExceptionHandler(this IApplicationBuilder app) =>
           app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}