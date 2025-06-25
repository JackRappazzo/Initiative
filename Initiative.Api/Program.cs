using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Identity.Roles;
using Initiative.Api.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Initiative.Persistence.Repositories;
using Initiative.Api.Core.Services.Encounters;
using Initiative.Persistence.Configuration;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Initiative.Api.Core.Utilities;
using Initiative.Api.Core.Services.Authentication;
using Initiative.Api.Core.Services.Users;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

builder.Services.AddScoped<IDatabaseConnectionFactory, DatabaseConnectionFactory>();
builder.Services.AddScoped<IJwtRefreshTokenRepository, JwtRefreshTokenRepository>();
builder.Services.AddScoped<IEncounterRepository, EncounterRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInitiativeUserRepository, InitiativeUserRepository>();
builder.Services.AddScoped<IUserLoginService, UserLoginService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICredentialsFactory, CredentialsFactory>();

builder.Services.AddScoped<IJwtRefreshService, JwtRefreshService>();
builder.Services.AddScoped<IEncounterService, EncounterService>();
builder.Services.AddScoped<IBase62CodeGenerator, Base62CodeGenerator>();

builder.Services.AddScoped<IUserManager<ApplicationIdentity>, UserManagerFacade<ApplicationIdentity>>();






//Identity
builder.Services.AddIdentityMongoDbProvider<Initiative.Api.Core.Identity.ApplicationIdentity, DefaultRole>(identityOptions =>
{
    identityOptions.Password.RequireNonAlphanumeric = true;
    identityOptions.Password.RequiredUniqueChars = 1;
    identityOptions.Password.RequiredLength = 6;
    identityOptions.User.RequireUniqueEmail = true;
    identityOptions.User.AllowedUserNameCharacters = null;
},
mongoOptions =>
{
    mongoOptions.ConnectionString = "mongodb://localhost:27017/Initiative";
});


// Register JwtSettings from environment variables
builder.Services.Configure<JwtSettings>(options =>
{
    options.Secret = JwtService.GetSecret(EnvironmentType.Local);
    options.Issuer = builder.Configuration["JwtSettings:Issuer"];
    options.Audience = builder.Configuration["JwtSettings:Audience"];
    options.ExpiresInMinutes = int.Parse(builder.Configuration["JwtSettings:ExpiresInMinutes"] ?? "60");
});

builder.Services.AddAuthentication(authOptions =>
{
    authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(jwtOptions =>
    {
        var secret = JwtService.GetSecret(EnvironmentType.Local);
        var key = Encoding.UTF8.GetBytes(secret);

        jwtOptions.RequireHttpsMetadata = false;
        jwtOptions.SaveToken = true;
        jwtOptions.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
        };
        jwtOptions.Events = new JwtBearerEvents()
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT FAILED: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT Validated Successfully");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                Console.WriteLine("Message received");
                return Task.CompletedTask;
            }
        };
    });


builder.Services.AddControllers().AddNewtonsoftJson();

var app = builder.Build();


// Configure the HTTP request pipeline.

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

app.Run();
