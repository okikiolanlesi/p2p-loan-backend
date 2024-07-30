using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.AspNetCore;
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

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
});

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
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Append("Token-Expired", "true");
                    context.Response.StatusCode = 401;
                }
                return Task.CompletedTask;
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var responseBody = JsonConvert.SerializeObject(new { message = "Unauthorized" });
                await context.Response.WriteAsync(responseBody);
                return;
            }
        };

    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

builder.Services
.AddFluentValidationAutoValidation();

builder.Services.AddHttpClient<MonnifyClient>().AddHttpMessageHandler(() => new P2PLoan.Clients.TokenHandler("/api/v1/auth/login", builder.Configuration["Monnify:APIKey"], builder.Configuration["Monnify:SecretKey"]))
;

// Seeders
builder.Services.AddScoped<P_1_UserSeeder>();
builder.Services.AddScoped<P_2_ModuleSeeder>();
builder.Services.AddScoped<P_3_WalletProviderSeeder>();
builder.Services.AddScoped<P_4_PermissionSeeder>();
builder.Services.AddScoped<P_5_RoleSeeder>();


builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
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
builder.Services.AddScoped<MonnifyWalletProviderService>(); // Ensure specific wallet provider services are registered
builder.Services.AddScoped<IWalletProviderServiceFactory, WalletProviderServiceFactory>();
builder.Services.AddScoped<IWalletService, WalletService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

try
{
    await DbInitializer.InitDb(app);
}
catch (Exception e)
{
    Console.WriteLine(e);
}

app.Run();

