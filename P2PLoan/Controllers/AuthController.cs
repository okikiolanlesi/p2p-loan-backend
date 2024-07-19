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
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthController(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IEmailService emailService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _configuration = configuration;
        _emailService = emailService;
    }

    // [HttpPost("register")]
    // public async Task<ActionResult> Register([FromBody] RegisterDto registerDto)
    // {

    // }

    // [HttpPost("login")]
    // public async Task<ActionResult> Login(LoginDto loginDto)
    // {


    // }

    // [HttpGet("verify-email")]
    // public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
    // {

    // }

    // [HttpPost]
    // [Route("forgot-password")]
    // public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    // {

    // }

    // [HttpPost]
    // [Route("reset-password")]
    // public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    // {

    // }

}