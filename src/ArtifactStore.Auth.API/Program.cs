using System.Text;
using ArtifactStore.Auth.API.Identity;
using ArtifactStore.Auth.API.Middleware;
using ArtifactStore.Auth.API.Services;
using ArtifactStore.Auth.Application;
using ArtifactStore.Auth.Application.Interfaces.Identity;
using ArtifactStore.Auth.Application.Interfaces.Services;
using ArtifactStore.Auth.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;

var logPath = Environment.GetEnvironmentVariable("AUTH_API_SERVER_LOGPATH") ?? "/app/logs/";
logPath += Environment.GetEnvironmentVariable("AUTH_API_SERVER_LOGFILENAME") ?? "ArtifactStore_Auth.log";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logPath)
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddSerilog();

    builder.Services.AddOpenApi();
    builder.Services.AddScoped<IIdentityHolder, IdentityHolder>();
    builder.Services.AddScoped<ITokenService, JwtTokenService>();
    builder.Services.AddControllers();
    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["SecretKey"]!)),

                ValidateIssuer = true,
                ValidIssuers = ["web-server"],

                ValidateAudience = true,
                ValidAudiences = ["web"],

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),

                NameClaimType = "name",
                RoleClaimType = "role"
            };
        });

    builder.Services.AddAuthorization();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseMiddleware<LoggerMiddleware>();
    app.UseMiddleware<ExceptionHandlingMiddleware>();
    app.UseAuthentication();
    app.UseAuthorization();
    app.UseMiddleware<UserInfoCollectionMiddleware>();
    app.MapControllers();

    app.UseHttpsRedirection();

    app.Run();
}
catch (Exception e)
{
    Log.Fatal(e, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
