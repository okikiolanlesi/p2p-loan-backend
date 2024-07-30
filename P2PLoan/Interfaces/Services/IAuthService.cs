using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces;

public interface IAuthService
{
    Task<ServiceResponse<object>> Register(RegisterRequestDto registerDto);
    Task<ServiceResponse<object>> Login(LoginRequestDto loginDto);
    Task<ServiceResponse<object>> VerifyEmail(Guid userId, string token);
    Task<ServiceResponse<object>> ForgotPassword(ForgotPasswordRequestDto forgotPasswordDto);
    Task<ServiceResponse<object>> ResetPassword(ResetPasswordRequestDto resetPasswordDto);
    Task<ServiceResponse<object>> SendEmailVerificationEmail(string email);
}
