using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using P2PLoan.DTOs;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Repositories;
using P2PLoan.Services;

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

    [HttpGet("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
    {
        var response = await authService.VerifyEmail(userId, token);
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

}