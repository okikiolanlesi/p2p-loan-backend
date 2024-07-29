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
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Utils;

namespace P2PLoan;

public class AuthService
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;
    private readonly IEmailService emailService;
    private readonly IConstants constants;
    private readonly IWalletService walletService;
    private readonly IWalletRepository walletRepository;
    private readonly IWalletProviderRepository walletProviderRepository;

    public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IEmailService emailService, IConstants constants, IWalletService walletService, IWalletRepository walletRepository, IWalletProviderRepository walletProviderRepository)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.configuration = configuration;
        this.emailService = emailService;
        this.constants = constants;
        this.walletService = walletService;
        this.walletRepository = walletRepository;
        this.walletProviderRepository = walletProviderRepository;
    }

    public async Task<ApiResponse<object>> Register(RegisterRequestDto registerDto)
    {
        // Start a transaction
        using (var transaction = await userRepository.BeginTransactionAsync())
        {
            try
            {
                // Checking if user doesn't already exists with that email
                var alreadyExistingUser = await userRepository.FindByEmail(registerDto.Email);

                if (alreadyExistingUser != null && alreadyExistingUser.EmailConfirmed)
                {
                    return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.AlreadyExists, "User already exists", null);
                }

                //Check if wallet provider exists
                var walletProvider = await walletProviderRepository.FindById(registerDto.WalletProviderId);

                if (walletProvider is null || !walletProvider.Enabled)
                {
                    return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.ResourceNotFound, "Invalid Wallet Provider Selected", null);
                }

                // Try to verify bvn
                var bvnIsVerified = await VerifyBVN(registerDto);

                if (!bvnIsVerified)
                {
                    return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.BvnNotVerified, "Unable to verify bvn details", null);
                }

                User user;

                if (alreadyExistingUser is null)
                {
                    // Map from dto to user model
                    user = mapper.Map<User>(registerDto);

                    user.Id = new Guid();

                    // Hashing the password before saving in the database
                    user.Password = HashPassword(registerDto.Password);
                }
                else
                {
                    // Doing this so that users without verified emails are overwritten
                    user = alreadyExistingUser;
                }

                // Set email verification data
                var emailVerificationToken = RandomCharacterGenerator.GenerateRandomString(constants.EMAIL_VERIFICATION_TOKEN_LENGTH);
                user.EmailVerificationToken = emailVerificationToken;
                user.EmailVerificationTokenExpiration = DateTime.UtcNow.AddMinutes(constants.EMAIL_VERIFICATION_TOKEN_EXPIRATION_MINUTES);

                if (alreadyExistingUser is null)
                {
                    // Saving user
                    userRepository.Add(user);
                }
                else
                {
                    // Modifying user
                    userRepository.MarkAsModified(user);
                }

                // Committing user changes
                var userSaveResult = await userRepository.SaveChangesAsync();
                if (!userSaveResult)
                {
                    throw new Exception();
                }

                // Start creating a wallet for the user

                //try parsing the date of birth into the right format
                string bvnDateOfBirth;
                try
                {
                    bvnDateOfBirth = DateConverter.ConvertIsoToDate(registerDto.BvnDateOfBirth);
                }
                catch
                {
                    return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.ValidationError, "Invalid BVN date of birth format", null);
                }

                var walletReferenceId = new Guid();
                CreateWalletDto createWalletDto = new CreateWalletDto
                {
                    WalletReference = $"{walletReferenceId}",
                    WalletName = $"P2PLoan Wallet - {user.Id}",
                    CustomerName = $"{user.FirstName} {user.LastName}",
                    BVNDetails = new BVNDetails
                    {
                        Bvn = registerDto.BVN,
                        BvnDateOfBirth = bvnDateOfBirth
                    },
                    CustomerEmail = user.Email
                };

                var createdWalletInfo = await walletService.Create(WalletProviders.monnify, createWalletDto);

                if (!createdWalletInfo.Created)
                {
                    //TODO: return response based on create wallet response
                }

                var wallet = new Wallet
                {
                    Id = new Guid(),
                    UserId = user.Id,
                    WalletProviderId = walletProvider.Id,
                    AccountNumber = createdWalletInfo.AccountNumber,
                    ReferenceId = $"{walletReferenceId}",
                };

                walletRepository.Add(wallet);


                // Committing changes
                var walletSaveResult = await walletRepository.SaveChangesAsync();

                if (!walletSaveResult)
                {
                    throw new Exception();
                }

                // Sending verification mail to newly registered user
                try
                {
                    await emailService.SendHtmlEmailAsync(user.Email, "Verify your email", "VerifyEmail", new
                    {
                        FrontendBaseUrl = configuration.GetSection("AppSettings:FrontendBaseUrl").Value,
                        Name = user.FirstName,
                        EmailVerificationToken = emailVerificationToken,
                        UserId = user.Id
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("Failed to send verification email");
                }


                return new ApiResponse<object>(ResponseStatus.Success, StatusCodes.Success, "Registered successfully", new
                {
                    user = mapper.Map<UserDto>(user)
                });
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();
                Console.WriteLine(ex);
                return new ApiResponse<object>(ResponseStatus.Error, StatusCodes.InternalServerError, "An unexpected error occurred", null);
            }
        }
    }

    public async Task<ApiResponse<object>> Login(LoginRequestDto loginDto)
    {

        var user = await userRepository.FindByEmail(loginDto.Email);

        if (user is null)
        {
            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.InvalidCredentials, "Invalid credentials", null);
        }

        // Verifying if password provided matches the saved hashed password
        if (!VerifyPassword(loginDto.Password, user.Password))
        {
            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.InvalidCredentials, "Invalid credentials", null);
        };

        if (!user.EmailConfirmed)
        {
            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.EmailNotVerified, "Email yet to be verified", null);
        }

        // Creating JWT
        string token = CreateJwtToken(user);

        var userDto = mapper.Map<UserDto>(user);

        return new ApiResponse<object>(ResponseStatus.Success, StatusCodes.Success, "Login Successful", new
        {
            message = "Login successfull",
            token,
            user = userDto
        });

    }

    public async Task<ApiResponse<object>> VerifyEmail(Guid userId, string token)
    {
        var user = await userRepository.GetByIdAsync(userId);

        if (user == null || user.EmailVerificationToken is null || user.EmailVerificationToken != token)
        {
            // Invalid or expired token
            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.InvalidVerificationToken, "Invalid verification token.", null);
        }

        if (user.EmailVerificationTokenExpiration is null || user.EmailVerificationTokenExpiration < DateTime.UtcNow)
        {
            // Invalid or expired token
            userRepository.Remove(user);

            await userRepository.SaveChangesAsync();

            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.InvalidVerificationToken, "Expired verification token, please register again to verify your email", null);
        }

        user.EmailConfirmed = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiration = null;

        // Save the updated user to the database
        userRepository.MarkAsModified(user);

        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ApiResponse<object>(ResponseStatus.Error, StatusCodes.InternalServerError, "Something went wrong", null);
        }

        return new ApiResponse<object>(ResponseStatus.Success, StatusCodes.Success, "Email verification successful. You can now log in.", new
        {
            user = mapper.Map<UserDto>(user)
        });
    }

    public async Task<ApiResponse<object>> ForgotPassword(ForgotPasswordRequestDto forgotPasswordDto)
    {
        var user = await userRepository.FindByEmail(forgotPasswordDto.Email);

        if (user == null)
        {
            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.InvalidCredentials, "User does not exist", null);
        }

        // Generate a unique token for resetting password
        var resetToken = RandomCharacterGenerator.GenerateRandomString(constants.PASSWORD_RESET_TOKEN_LENGTH);
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiration = DateTime.UtcNow.AddMinutes(constants.PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES); // Token expiration time

        userRepository.MarkAsModified(user);
        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ApiResponse<object>(ResponseStatus.Error, StatusCodes.InternalServerError, "Something went wrong", null);
        }

        // Send the reset link via email
        await emailService.SendHtmlEmailAsync(user.Email, "Reset Password", "ResetPassword", new
        {
            FrontendBaseUrl = configuration.GetSection("AppSettings:FrontendBaseUrl").Value,
            Name = user.FirstName,
            ResetToken = resetToken,
            user.Email,
        });

        return new ApiResponse<object>(ResponseStatus.Success, StatusCodes.Success, "Password reset email sent", null);
    }

    public async Task<ApiResponse<object>> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordDto)
    {
        var user = await userRepository.FindByEmail(resetPasswordDto.Email);

        if (user == null || user.PasswordResetTokenExpiration < DateTime.UtcNow)
        {
            // Token is invalid or expired
            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.InvalidVerificationToken, "Invalid or expired token", null);
        }

        // Reset password
        user.Password = HashPassword(resetPasswordDto.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiration = null;

        userRepository.MarkAsModified(user);
        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ApiResponse<object>(ResponseStatus.Error, StatusCodes.InternalServerError, "Something went wrong", null);
        }

        return new ApiResponse<object>(ResponseStatus.Success, StatusCodes.Success, "Password reset successful", null);
    }

    private string CreateJwtToken(User user)
    {

        // Declaring claims we would like to write to the JWT
        List<Claim> claims = new List<Claim>{
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            // new Claim(ClaimTypes.Role, user.Role.ToString())

        };

        // Creating a new SymmetricKey from Token we have saved in appSettings.development.json file
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("JWT:Token").Value));

        // Declaring signing credentials
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        // Creating new JWT object
        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        // Write JWT to a string
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        return jwt;
    }

    private string HashPassword(string password)
    {

        return BCrypt.Net.BCrypt.HashPassword(password);
    }
    private bool VerifyPassword(string password, string passwordHash)
    {

        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    private async Task<bool> VerifyBVN(RegisterRequestDto registerRequestDto)
    {
        // TODO: Implement logic to verify BVN info
        // Simulating an async operation for demonstration purposes
        await Task.Delay(1); // Simulate an asynchronous operation

        return true;
    }

    public async Task<ApiResponse<object>> SendEmailVerificationEmail(string email)
    {

        // Checking if user exists with that email
        var user = await userRepository.FindByEmail(email);

        if (user is null)
        {
            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.InvalidCredentials, "Invalid credentials", null);
        }

        if (user != null && user.EmailConfirmed)
        {
            return new ApiResponse<object>(ResponseStatus.BadRequest, StatusCodes.AlreadyExists, "Email already verified", null);
        }

        // Set email verification data
        var emailVerificationToken = RandomCharacterGenerator.GenerateRandomString(constants.EMAIL_VERIFICATION_TOKEN_LENGTH);
        user.EmailVerificationToken = emailVerificationToken;
        user.EmailVerificationTokenExpiration = DateTime.UtcNow.AddMinutes(constants.EMAIL_VERIFICATION_TOKEN_EXPIRATION_MINUTES);


        // Sending verification mail to newly registered user
        try
        {
            await emailService.SendHtmlEmailAsync(user.Email, "Verify your email", "VerifyEmail", new
            {
                FrontendBaseUrl = configuration.GetSection("AppSettings:FrontendBaseUrl").Value,
                Name = user.FirstName,
                EmailVerificationToken = emailVerificationToken,
                UserId = user.Id
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Failed to send verification email");

            return new ApiResponse<object>(ResponseStatus.Error, StatusCodes.InternalServerError, "Unable to send verification email", null);
        }

        return new ApiResponse<object>(ResponseStatus.Success, StatusCodes.Success, "Verification email successfully", null);
    }
}
