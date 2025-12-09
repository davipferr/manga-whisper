using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MangaWhisper.Infrastructure.Data;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Factories;
using MangaWhisper.Domain.Repositories;
using MangaWhisper.Infrastructure.Services;
using MangaWhisper.Infrastructure.Repositories;

namespace MangaWhisper.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get connection string from configuration (appsettings.json or Azure App Settings)
        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
                              ?? throw new InvalidOperationException("Database connection string 'DefaultConnection' not found in configuration.");

        // Entity Framework DbContext for write operations (Commands)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Configure Identity
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
        })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

        // Configure JWT Authentication
        var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET")
                       ?? throw new InvalidOperationException("JWT_SECRET environment variable not found.");
        var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER")
                        ?? throw new InvalidOperationException("JWT_ISSUER environment variable not found.");
        var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE")
                        ?? throw new InvalidOperationException("JWT_AUDIENCE environment variable not found.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ClockSkew = TimeSpan.Zero
            };
        });

        // Query DbContext for read operations (Dapper)
        services.AddScoped<QueryDbContext>(provider =>
            new QueryDbContext(connectionString));

        // HttpClient
        services.AddHttpClient();

        // Infrastructure specific services
        services.AddScoped<IChapterCheckerFactory, ChapterCheckerFactory>();
        services.AddScoped<IMangaCheckerRepository, MangaCheckerRepository>();
        services.AddScoped<IChapterRepository, ChapterRepository>();
        services.AddScoped<IMangaRepository, MangaRepository>();

        // Background service
        services.AddHostedService<ChapterCheckingBackgroundService>();

        return services;
    }
}
