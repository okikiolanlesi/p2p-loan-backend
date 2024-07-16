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

    //     // Checking if password is equal to passwordConfirm
    //     var passwordMatch = registerDto.Password == registerDto.ConfirmPassword;

    //     if (!passwordMatch) return BadRequest(new
    //     {
    //         message = "Password mismatch"
    //     });

    //     // Checking if user doesn't already exists with that email
    //     var isAlreadyExists = await _userRepository.GetByEmail(registerDto.Email);

    //     if (isAlreadyExists != null && isAlreadyExists.EmailConfirmed)
    //     {
    //         return BadRequest(new
    //         {
    //             message = "User already exists",
    //         });
    //     }

    //     User user;

    //     if (isAlreadyExists is null)
    //     {
    //         // Map from dto to user model
    //         user = _mapper.Map<User>(registerDto);

    //         // Hashing the password before saving in the database
    //         user.PasswordHash = HashPassword(registerDto.Password);
    //     }
    //     else
    //     {
    //         user = isAlreadyExists;
    //     }

    //     // TODO: Remove this line when I get email to work in  production
    //     user.EmailConfirmed = true;


    //     // Set email verification data
    //     var emailVerificationToken = GenerateUniqueToken();
    //     user.EmailVerificationToken = emailVerificationToken;
    //     user.EmailVerificationTokenExpiration = DateTime.UtcNow.AddHours(24);



    //     if (isAlreadyExists is null)
    //     {
    //         // Saving user
    //         _userRepository.Add(user);
    //     }
    //     else
    //     {
    //         // Modifying user
    //         _userRepository.MarkAsModified(user);
    //     }


    //     // Committing changes
    //     var result = await _userRepository.SaveChangesAsync();

    //     // Sending verification mail to newly registered user
    //     try
    //     {
    //         await _emailService.SendHtmlEmailAsync(user.Email, "Verify your email", "VerifyEmail", new
    //         {
    //             FrontendBaseUrl = _configuration.GetSection("AppSettings:FrontendBaseUrl").Value,
    //             Name = user.FirstName,
    //             EmailVerificationToken = emailVerificationToken,
    //             UserId = user.Id
    //         });
    //     }
    //     catch (Exception e)
    //     {
    //         Console.WriteLine(e);
    //         Console.WriteLine("Failed to send verification email");
    //     }

    //     if (!result) return Problem(title: "Something went wrong");

    //     return Ok(new
    //     {
    //         message = "User registered successfully",
    //         token = CreateJwtToken(user),
    //         user

    //     });
    // }

    // [HttpPost("login")]
    // public async Task<ActionResult> Login(LoginDto loginDto)
    // {

    //     var user = await _userRepository.GetByEmail(loginDto.Email);

    //     if (user is null)
    //     {
    //         return BadRequest(new
    //         {
    //             message = "Invalid credentials"
    //         });
    //     }

    //     // Verifying if password provided matches the saved hashed password
    //     if (!VerifyPassword(loginDto.Password, user.PasswordHash))
    //     {
    //         return BadRequest(new
    //         {
    //             message = "Invalid credentials"
    //         });
    //     };

    //     // if (!user.EmailConfirmed)
    //     // {
    //     //     return BadRequest(new
    //     //     {
    //     //         message = "Email yet to be verified"
    //     //     });
    //     // }


    //     // Creating JWT
    //     string token = CreateJwtToken(user);

    //     var userdto = _mapper.Map<UserDto>(user);

    //     return Ok(new
    //     {
    //         message = "Login successfull",
    //         token,
    //         user = userdto
    //     });

    // }

    // [HttpGet("verify-email")]
    // public async Task<IActionResult> VerifyEmail([FromQuery] Guid userId, [FromQuery] string token)
    // {
    //     var user = await _userRepository.GetById(userId);

    //     if (user == null || user.EmailVerificationToken != token)
    //     {
    //         // Invalid or expired token
    //         return BadRequest(new { message = "Invalid verification token." });
    //     }

    //     if (user.EmailVerificationTokenExpiration < DateTime.UtcNow)
    //     {
    //         // Invalid or expired token
    //         _userRepository.DeleteUser(user.Id);

    //         return BadRequest(new { message = "Expired verification token, please register again to verify your email" });
    //     }

    //     user.EmailConfirmed = true;
    //     user.EmailVerificationToken = null;
    //     user.EmailVerificationTokenExpiration = null;

    //     // Save the updated user to the database
    //     _userRepository.MarkAsModified(user);

    //     var result = await _userRepository.SaveChangesAsync();
    //     if (result == false)
    //     {
    //         return Problem("Something went wrong");
    //     }

    //     return Ok(new { message = "Email verification successful. You can now log in.", token = CreateJwtToken(user), user = _mapper.Map<UserDto>(user) });
    // }

    // [HttpPost]
    // [Route("forgot-password")]
    // public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    // {
    //     var user = await _userRepository.GetByEmail(forgotPasswordDto.Email);

    //     if (user == null)
    //     {
    //         return Ok();
    //     }

    //     // Generate a unique token for resetting password
    //     var resetToken = GenerateUniqueToken();
    //     user.ResetToken = resetToken;
    //     user.ResetTokenExpiration = DateTime.UtcNow.AddHours(1); // Token expiration time

    //     _userRepository.MarkAsModified(user);
    //     var result = await _userRepository.SaveChangesAsync();
    //     if (result == false)
    //     {
    //         return Problem("Something went wrong");
    //     }

    //     // Send the reset link via email
    //     _emailService.SendHtmlEmailAsync(user.Email, "Reset Password", "ResetPassword", new
    //     {
    //         FrontendBaseUrl = _configuration.GetSection("AppSettings:FrontendBaseUrl").Value,
    //         Name = user.FirstName,
    //         ResetToken = resetToken
    //     });

    //     return Ok(new
    //     {
    //         message = "Password reset email sent"
    //     });
    // }

    // [HttpPost]
    // [Route("reset-password")]
    // public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    // {
    //     var user = await _userRepository.GetUserByResetToken(resetPasswordDto.Token);

    //     if (user == null || user.ResetTokenExpiration < DateTime.UtcNow)
    //     {
    //         // Token is invalid or expired
    //         return BadRequest("Invalid or expired token");
    //     }

    //     // Reset password
    //     user.PasswordHash = HashPassword(resetPasswordDto.NewPassword);
    //     user.ResetToken = null;
    //     user.ResetTokenExpiration = null;

    //     _userRepository.MarkAsModified(user);
    //     var result = await _userRepository.SaveChangesAsync();
    //     if (result == false)
    //     {
    //         return Problem("Something went wrong");
    //     }

    //     return Ok(new
    //     {
    //         message = "Password reset succesfully"
    //     });
    // }

    // private string CreateJwtToken(User user)
    // {

    //     // Declaring claims we would like to write to the JWT
    //     List<Claim> claims = new List<Claim>{
    //         new Claim(ClaimTypes.Email, user.Email),
    //         new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    //         new Claim(ClaimTypes.Role, user.Role.ToString())

    //     };

    //     // Creating a new SymmetricKey from Token we have saved in appSettings.development.json file
    //     var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

    //     // Declaring signing credentials
    //     var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

    //     // Creating new JWT object
    //     var token = new JwtSecurityToken(
    //         claims: claims,
    //         expires: DateTime.Now.AddDays(1),
    //         signingCredentials: creds
    //     );

    //     // Write JWT to a string
    //     var jwt = new JwtSecurityTokenHandler().WriteToken(token);

    //     return jwt;
    // }

    // private string GenerateUniqueToken()
    // {
    //     return Guid.NewGuid().ToString();
    // }

    // private string HashPassword(string password)
    // {

    //     return BCrypt.Net.BCrypt.HashPassword(password);
    // }
    // private bool VerifyPassword(string password, string passwordHash)
    // {

    //     return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    // }
}