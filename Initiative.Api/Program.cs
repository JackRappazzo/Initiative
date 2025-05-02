using Initiative.Api.Services;
using Microsoft.AspNetCore.Identity;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Model;
using Initiative.Api.Core.Identity;
using Initiative.Api.Core.Identity.Roles;
using Initiative.Api.Core.Authentication;
using Initiative.Api.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();


builder.Services.AddScoped<IUserRegistrationService, UserRegistrationService>();
builder.Services.AddScoped<IUserLoginService, UserLoginService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICredentialsFactory, CredentialsFactory>();

//Identity
builder.Services.AddIdentityMongoDbProvider<InitiativeUser, DefaultRole>(identityOptions =>
{
    identityOptions.Password.RequireNonAlphanumeric = true;
    identityOptions.Password.RequiredUniqueChars = 1;
    identityOptions.Password.RequiredLength = 6;
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


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
