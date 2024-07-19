using System;

namespace P2PLoan.Helpers;

public class ApiResponse<T>
{
    public ResponseStatus Status { get; set; }
    public string StatusCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public T? Result { get; set; } = default(T);
    public ApiResponse(ResponseStatus status, StatusCodes statusCode, string message, T? result)
    {
        Status = status;
        Message = message;
        Result = result;
        StatusCode = ((int)statusCode).ToString("D4");
    }

}
