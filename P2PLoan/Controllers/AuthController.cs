using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using P2PLoan.Attributes;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;

namespace P2PLoan.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;
    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerDto)
    {
        var response = await authService.Register(registerDto);
        return ControllerHelper.HandleApiResponse(response);

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto loginDto)
    {
        var response = await authService.Login(loginDto);
        return ControllerHelper.HandleApiResponse(response);

    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail(VerifyEmailRequestDto verifyEmailDto)
    {
        var response = await authService.VerifyEmail(verifyEmailDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost]
    [Route("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto forgotPasswordDto)
    {
        var response = await authService.ForgotPassword(forgotPasswordDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost]
    [Route("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordDto)
    {
        var response = await authService.ResetPassword(resetPasswordDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPost]
    [Route("create-pin")]
    [Authorize]
    public async Task<IActionResult> CreatePin([FromBody] CreatePinRequestDto createPinRequestDtoPasswordDto)
    {
        var response = await authService.CreatePin(createPinRequestDtoPasswordDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPatch]
    [Route("change-pin")]
    [Authorize]
    public async Task<IActionResult> ChangePin([FromBody] ChangePinRequestDto changePinRequestDto)
    {
        var response = await authService.ChangePin(changePinRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpPatch]
    [Route("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordRequestDto)
    {
        var response = await authService.ChangePassword(changePasswordRequestDto);
        return ControllerHelper.HandleApiResponse(response);
    }

    [HttpGet]
    [Route("test")]
    // [Authorize]
    [Permission(Modules.user, PermissionAction.create, [UserType.borrower])]
    public async Task<IActionResult> Test()
    {
        return Ok("Working");
    }

    [HttpPost]
    [Route("resend-email-verification")]
    public async Task<IActionResult> ResendVerificationEmail(ResendVerificationEmailDto dto)
    {
        var response = await authService.ResendEmailVerification(dto.Email);
        return ControllerHelper.HandleApiResponse(response);
    }

}