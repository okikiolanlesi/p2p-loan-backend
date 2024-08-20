using System;
using System.IO;
using System.Linq;
using System.Text;
// using FluentValidation;
// using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using P2PLoan;
using P2PLoan.Clients;
using P2PLoan.Data;
using P2PLoan.Interfaces;
using P2PLoan.Repositories;
using P2PLoan.Seeders;
using P2PLoan.Services;
using P2PLoan.Validators;
using Azure.Identity;
using Microsoft.AspNetCore.Authorization;
using P2PLoan.Helpers;
using System.Text.Json.Serialization;
using System.Text.Json;
using P2PLoan.Providers;
using P2PLoan.Constants;
using P2PLoan.Models;
using P2PLoan.Requirements;
using P2PLoan.Handlers;
using P2PLoan.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Configuration variables (retrieved from environment variables)
var tenantId = builder.Configuration["AZURE_TENANT_ID"];
var clientId = builder.Configuration["AZURE_CLIENT_ID"];
var clientSecret = builder.Configuration["AZURE_CLIENT_SECRET"];
var vaultUri = new Uri(builder.Configuration["AZURE_VAULT_URI"]);

// Create ClientSecretCredential
var clientSecretCredential = new ClientSecretCredential(tenantId, clientId, clientSecret);

// Add Azure Key Vault to configuration
builder.Configuration.AddAzureKeyVault(vaultUri, clientSecretCredential);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Register SimpleLogger



builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .WithMethods("GET", "POST", "PATCH", "PUT", "DELETE")
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials();
        });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }).AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter { AllowIntegerValues = true });
        }); ;

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async context =>
            {
                // This should handle cases like invalid tokens or malformed tokens.
                if (context.Exception is SecurityTokenExpiredException)
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var responseBody = JsonConvert.SerializeObject(new ServiceResponse<object>(ResponseStatus.Unauthorized, AppStatusCodes.Unauthorized, "Token expired", null));
                    await context.Response.WriteAsync(responseBody);
                }
                else
                {
                    context.Response.Headers.Append("Authentication-Failed", "true");
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var responseBody = JsonConvert.SerializeObject(new ServiceResponse<object>(ResponseStatus.Unauthorized, AppStatusCodes.Unauthorized, "Authentication failed", null));
                    await context.Response.WriteAsync(responseBody);
                }
                return;
            },
            OnChallenge = async context =>
            {
                // This should handle cases where no authentication was provided.
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var responseBody = JsonConvert.SerializeObject(new ServiceResponse<object>(ResponseStatus.Unauthorized, AppStatusCodes.Unauthorized, "Unauthorized", null));
                await context.Response.WriteAsync(responseBody);
                return;
            },
            OnForbidden = async context =>
            {
                // This handles cases where authentication succeeded but the user lacks permissions.
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var responseBody = JsonConvert.SerializeObject(new ServiceResponse<object>(ResponseStatus.Forbidden, AppStatusCodes.Unauthorized, "You are not allowed to use this feature", null));
                await context.Response.WriteAsync(responseBody);
                return;
            }
        };

    });


builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("PermissionPolicy", policy =>
            {
                policy.Requirements.Add(new PermissionRequirement(Modules.notification, PermissionAction.create, [UserType.borrower])); // Dummy requirement to force the handler to run
            });
        });

builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddHttpContextAccessor();
;

builder.Services.AddDbContext<P2PLoanDbContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerDatabase"));
});

builder.Services.AddSingleton<IEmailService>(provider =>
{
    var logger = provider.GetRequiredService<ILogger<EmailService>>();
    var environment = provider.GetRequiredService<IWebHostEnvironment>();

    var templatesFolderPath = Path.Combine(environment.ContentRootPath, "Emails");

    return new EmailService(templatesFolderPath, logger, builder.Configuration);
});

// builder.Services
// .AddFluentValidationAutoValidation();

builder.Services.AddHttpClient<MonnifyClient>().AddHttpMessageHandler(() => new P2PLoan.Clients.TokenHandler("/api/v1/auth/login", builder.Configuration["Monnify:APIKey"], builder.Configuration["Monnify:SecretKey"], builder.Configuration["Monnify:BaseUrl"]))
;

builder.Services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider>();


// Seeders
builder.Services.AddScoped<P_1_UserSeeder>();
builder.Services.AddScoped<P_2_ModuleSeeder>();
builder.Services.AddScoped<P_3_WalletProviderSeeder>();
builder.Services.AddScoped<P_4_PermissionSeeder>();
builder.Services.AddScoped<P_5_RoleSeeder>();


// builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IModuleRepository, ModuleRepository>();
builder.Services.AddScoped<ISeedRepository, SeedRepository>();
builder.Services.AddScoped<IWalletProviderRepository, WalletProviderRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddScoped<IConstants, Constants>();
builder.Services.AddScoped<ISeederHandler, SeederHandler>();
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IMonnifyApiService, MonnifyApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<MonnifyWalletProviderService>(); // Ensure specific wallet provider services are registered
builder.Services.AddScoped<IWalletProviderServiceFactory, WalletProviderServiceFactory>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Add the exception handling middleware to the pipeline
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();
app.UseCustomExceptionHandler();

try
{
    await DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();

