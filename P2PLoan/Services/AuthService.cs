using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using P2PLoan.Constants;
using P2PLoan.DTOs;
using P2PLoan.Helpers;
using P2PLoan.Interfaces;
using P2PLoan.Models;
using P2PLoan.Utils;

namespace P2PLoan.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository userRepository;
    private readonly IMapper mapper;
    private readonly IConfiguration configuration;
    private readonly IEmailService emailService;
    private readonly IConstants constants;
    private readonly IWalletService walletService;
    private readonly IWalletRepository walletRepository;
    private readonly IWalletProviderRepository walletProviderRepository;
    private readonly IHttpContextAccessor httpContextAccessor;

    public AuthService(IUserRepository userRepository, IMapper mapper, IConfiguration configuration, IEmailService emailService, IConstants constants, IWalletService walletService, IWalletRepository walletRepository, IWalletProviderRepository walletProviderRepository, IHttpContextAccessor httpContextAccessor)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
        this.configuration = configuration;
        this.emailService = emailService;
        this.constants = constants;
        this.walletService = walletService;
        this.walletRepository = walletRepository;
        this.walletProviderRepository = walletProviderRepository;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<ServiceResponse<object>> Register(RegisterRequestDto registerDto)
    {

        if (registerDto.UserType != UserType.borrower && registerDto.UserType != UserType.lender)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Invalid user type", null);
        }

        // Start a transaction
        using (var transaction = await userRepository.BeginTransactionAsync())
        {
            try
            {
                // Checking if user doesn't already exists with that email
                var alreadyExistingUser = await userRepository.GetByEmailAsync(registerDto.Email);

                if (alreadyExistingUser != null && alreadyExistingUser.EmailConfirmed)
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.AlreadyExists, "User already exists", null);
                }

                //Check if wallet provider exists
                var walletProvider = await walletProviderRepository.FindById(registerDto.WalletProviderId);

                if (walletProvider is null || !walletProvider.Enabled)
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "Invalid Wallet Provider Selected", null);
                }

                // Try to verify bvn
                var bvnIsVerified = await VerifyBVN(registerDto);

                if (!bvnIsVerified)
                {
                    return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.BvnNotVerified, "Unable to verify bvn details", null);
                }

                User user;

                if (alreadyExistingUser is null)
                {
                    // Map from dto to user model
                    user = mapper.Map<User>(registerDto);

                    user.Id = Guid.NewGuid();

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


                // Check if user already has a wallet
                if (alreadyExistingUser is null)
                {
                    // Start creating a wallet for the user

                    //try parsing the date of birth into the right format
                    string bvnDateOfBirth;
                    try
                    {
                        bvnDateOfBirth = DateConverter.ConvertIsoToDate(registerDto.BvnDateOfBirth);
                    }
                    catch
                    {
                        return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Invalid BVN date of birth format", null);
                    }


                    var walletReferenceId = Guid.NewGuid();
                    CreateWalletDto createWalletDto = new CreateWalletDto
                    {
                        WalletReference = $"{walletReferenceId}",
                        WalletName = $"P2PLoan Wallet - {user.Id}",
                        CustomerName = $"{user.FirstName} {user.LastName}",
                        BvnDetails = new BVNDetails
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
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        WalletProviderId = walletProvider.Id,
                        AccountNumber = createdWalletInfo.AccountNumber,
                        ReferenceId = $"{walletReferenceId}",
                        TopUpAccountName = createdWalletInfo.TopUpAccountName,
                        TopUpAccountNumber = createdWalletInfo.TopUpAccountNumber,
                        TopUpBankName = createdWalletInfo.TopUpBankName,
                        TopUpBankCode = createdWalletInfo.TopUpBankCode,
                        CreatedById = user.Id,
                        ModifiedById = user.Id,
                    };

                    walletRepository.Add(wallet);


                    // Committing changes
                    var walletSaveResult = await walletRepository.SaveChangesAsync();

                    if (!walletSaveResult)
                    {
                        throw new Exception();
                    }
                }

                // Sending verification mail to newly registered user
                await SendVerificationMail(user, emailVerificationToken);

                // Commit transaction if it gets to this point
                await transaction.CommitAsync();

                return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Registered successfully", new
                {
                    user = mapper.Map<UserDto>(user)
                });
            }
            catch (Exception ex)
            {
                // Rollback the transaction if any error occurs
                await transaction.RollbackAsync();
                Console.WriteLine(ex);
                return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "An unexpected error occurred", null);
            }
        }
    }

    public async Task<ServiceResponse<object>> Login(LoginRequestDto loginDto)
    {

        var user = await userRepository.GetByEmailAsync(loginDto.Email);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidCredentials, "Invalid credentials", null);
        }

        // Verifying if password provided matches the saved hashed password
        if (!VerifyPassword(loginDto.Password, user.Password))
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidCredentials, "Invalid credentials", null);
        };

        if (!user.EmailConfirmed)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.EmailNotVerified, "Email yet to be verified", null);
        }

        // Creating JWT
        string token = CreateJwtToken(user);

        var userDto = mapper.Map<UserDto>(user);

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Login Successful", new
        {
            message = "Login successfull",
            token,
            user = userDto
        });

    }

    public async Task<ServiceResponse<object>> VerifyEmail(VerifyEmailRequestDto verifyEmailDto)
    {
        var user = await userRepository.GetByEmailAsync(verifyEmailDto.Email);

        if (user == null || user.EmailVerificationToken is null || user.EmailVerificationToken != verifyEmailDto.Token)
        {
            // Invalid or expired token
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidVerificationToken, "Invalid verification token.", null);
        }

        if (user.EmailVerificationTokenExpiration is null || user.EmailVerificationTokenExpiration < DateTime.UtcNow)
        {
            // Invalid or expired token
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidVerificationToken, "Expired verification token, please resend token and verify again", null);
        }

        user.EmailConfirmed = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpiration = null;

        // Save the updated user to the database
        userRepository.MarkAsModified(user);

        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Email verification successful. You can now log in.", new
        {
            user = mapper.Map<UserDto>(user)
        });
    }
    public async Task<ServiceResponse<object>> ResendEmailVerification(string email)
    {
        var user = await userRepository.GetByEmailAsync(email);

        if (user == null)
        {
            // Invalid or expired token
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidCredentials, "Invalid credentials", null);
        }
        if (user.EmailConfirmed)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidCredentials, "Email already verified", null);
        }

        // Restart the verification process
        user.EmailVerificationToken = RandomCharacterGenerator.GenerateRandomString(constants.EMAIL_VERIFICATION_TOKEN_LENGTH);

        user.EmailVerificationTokenExpiration = DateTime.UtcNow.AddMinutes(constants.EMAIL_VERIFICATION_TOKEN_EXPIRATION_MINUTES);

        // Save the updated user to the database
        userRepository.MarkAsModified(user);

        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
        }

        // Send the verification email
        await SendVerificationMail(user, user.EmailVerificationToken);

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Email sent successfully.", null);
    }

    public async Task<ServiceResponse<object>> ForgotPassword(ForgotPasswordRequestDto forgotPasswordDto)
    {
        var user = await userRepository.GetByEmailAsync(forgotPasswordDto.Email);

        if (user == null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidCredentials, "User does not exist", null);
        }

        // Generate a unique token for resetting password
        var resetToken = RandomCharacterGenerator.GenerateRandomString(constants.PASSWORD_RESET_TOKEN_LENGTH);
        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpiration = DateTime.UtcNow.AddMinutes(constants.PASSWORD_RESET_TOKEN_EXPIRATION_MINUTES); // Token expiration time

        userRepository.MarkAsModified(user);
        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
        }

        // Send the reset link via email
        await emailService.SendHtmlEmailAsync(user.Email, "Reset Password", "ResetPassword", new
        {
            FrontendBaseUrl = configuration.GetSection("AppSettings:FrontendBaseUrl").Value,
            Name = user.FirstName,
            ResetToken = resetToken,
            user.Email,
        });

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Password reset email sent", null);
    }

    public async Task<ServiceResponse<object>> ResetPassword([FromBody] ResetPasswordRequestDto resetPasswordDto)
    {
        var user = await userRepository.GetByEmailAsync(resetPasswordDto.Email);

        if (user == null || user.PasswordResetTokenExpiration < DateTime.UtcNow || user.PasswordResetToken != resetPasswordDto.Token)
        {
            // Token is invalid or expired
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidVerificationToken, "Invalid or expired token", null);
        }

        // Reset password
        user.Password = HashPassword(resetPasswordDto.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpiration = null;

        userRepository.MarkAsModified(user);
        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Password reset successful", null);
    }

    public async Task<ServiceResponse<object>> CreatePin([FromBody] CreatePinRequestDto createPinDto)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        // Check if user already created a pin
        if (user.PinCreated)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.PinAlreadyCreated, "You already have a pin", null);
        }

        // Check if pin match
        if (createPinDto.Pin != createPinDto.ConfirmPin)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Pin does not match", null);
        }

        //Validate that pin is in the right format
        var pinOnlyContainsNumbers = Utilities.IsStringNumericRegex(createPinDto.Pin);

        if (!pinOnlyContainsNumbers)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Invalid pin format", null);
        }

        // Create pin
        user.PIN = HashPassword(createPinDto.Pin);
        user.PinCreated = true;
        user.ModifiedAt = DateTime.UtcNow;

        userRepository.MarkAsModified(user);
        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Pin created successfully", null);
    }

    public async Task<ServiceResponse<object>> ChangePin([FromBody] ChangePinRequestDto changePinDto)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        // Check if user has created a pin
        if (!user.PinCreated)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.NoPinCreated, "User does not have a pin", null);
        }

        //Confirm if old pin is correct
        var isOldPinCorrect = VerifyPassword(changePinDto.OldPin, user.PIN);

        if (!isOldPinCorrect)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidCredentials, "Invalid credentials", null);

        }

        // Check if pin match
        if (changePinDto.NewPin != changePinDto.ConfirmNewPin)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Pin does not match", null);
        }

        //Validate that pin is in the right format
        var pinOnlyContainsNumbers = Utilities.IsStringNumericRegex(changePinDto.NewPin);

        if (!pinOnlyContainsNumbers)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Invalid pin format", null);
        }

        // Change pin
        user.PIN = HashPassword(changePinDto.NewPin);
        user.ModifiedAt = DateTime.UtcNow;

        userRepository.MarkAsModified(user);
        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Pin changed successfully", null);
    }

    public async Task<ServiceResponse<object>> ChangePassword([FromBody] ChangePasswordRequestDto changePasswordDto)
    {
        var userId = httpContextAccessor.HttpContext.User.GetLoggedInUserId();

        var user = await userRepository.GetByIdAsync(userId);

        if (user is null)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ResourceNotFound, "User does not exist", null);
        }

        //Confirm if old password is correct
        var isOldPasswordCorrect = VerifyPassword(changePasswordDto.OldPassword, user.Password);

        if (!isOldPasswordCorrect)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.InvalidCredentials, "Invalid credentials", null);

        }

        // Check if password matches
        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
        {
            return new ServiceResponse<object>(ResponseStatus.BadRequest, AppStatusCodes.ValidationError, "Password does not match", null);
        }

        // Change password
        user.Password = HashPassword(changePasswordDto.NewPassword);
        user.ModifiedAt = DateTime.UtcNow;

        userRepository.MarkAsModified(user);
        var result = await userRepository.SaveChangesAsync();
        if (!result)
        {
            return new ServiceResponse<object>(ResponseStatus.Error, AppStatusCodes.InternalServerError, "Something went wrong", null);
        }

        return new ServiceResponse<object>(ResponseStatus.Success, AppStatusCodes.Success, "Password changed successfully", null);
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


    private async Task SendVerificationMail(User user, string emailVerificationToken)
    {
        try
        {
            await emailService.SendHtmlEmailAsync(user.Email, "Verify your email", "VerifyEmail", new
            {
                FrontendBaseUrl = configuration.GetSection("AppSettings:FrontendBaseUrl").Value,
                Name = user.FirstName,
                EmailVerificationToken = emailVerificationToken,
                UserId = user.Id,
                Email = user.Email
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Console.WriteLine("Failed to send verification email");
        }
    }
}
