using System;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using P2PLoan.Constants;
using P2PLoan.Helpers;

namespace P2PLoan.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {

            await _next(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception occurred: {Message}", e.Message);

            // var exceptionDetails = GetExceptionDetails(e);

            context.Response.StatusCode = 500;

            await context.Response.WriteAsJsonAsync(new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "An unexpected error has occurred", null));
        }
    }

    // private static ServiceResponse<object> GetExceptionDetails(Exception exception)
    // {
    //     return exception switch
    //     {
    //         ValidationException validationException => new ServiceResponse<object>(ResponseStatus.BadRequest, StatusCodes.ValidationError, "Validation Error", null),
    //         _ => new ServiceResponse<object>(ResponseStatus.Error, StatusCodes.InternalServerError, "An unexpected error has occurred", null)
    //     };
    // }

    // private sealed class ValidationException : Exception
    // {
    //     public ValidationException(IEnumerable<ValidationError> errors)
    //     {
    //         Errors = errors;
    //     }

    //     public IEnumerable<ValidationError> Errors { get; }
    // }

    // private sealed record ValidationError(string PropertyName, string ErrorMessage);

}
