using System;
using System.Threading.Tasks;
using P2PLoan.DTOs;
using P2PLoan.Helpers;

namespace P2PLoan.Interfaces;

public interface IAuthService
{
    Task<ServiceResponse<object>> Register(RegisterRequestDto registerDto);
    Task<ServiceResponse<object>> Login(LoginRequestDto loginDto);
    Task<ServiceResponse<object>> VerifyEmail(VerifyEmailRequestDto verifyEmailRequestDto);
    Task<ServiceResponse<object>> ForgotPassword(ForgotPasswordRequestDto forgotPasswordDto);
    Task<ServiceResponse<object>> ResetPassword(ResetPasswordRequestDto resetPasswordDto);
    Task<ServiceResponse<object>> CreatePin(CreatePinRequestDto createPinRequestDto);
    Task<ServiceResponse<object>> ChangePin(ChangePinRequestDto changePinRequestDto);
    Task<ServiceResponse<object>> ForgotPin(ForgotPinRequestDto forgotPinRequestDto);
    Task<ServiceResponse<object>> ResetPin(ResetPinRequestDto resetPinRequestDto);
    Task<ServiceResponse<object>> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto);
    Task<ServiceResponse<object>> ResendEmailVerification(string email);
}
