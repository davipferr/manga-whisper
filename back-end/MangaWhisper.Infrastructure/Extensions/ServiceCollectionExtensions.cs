using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MangaWhisper.Infrastructure.Data;
using MangaWhisper.Domain.Factories;
using MangaWhisper.Application.Services;
using MangaWhisper.Infrastructure.Services;

namespace MangaWhisper.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Get connection string from .env file
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                              ?? throw new InvalidOperationException("Database connection string not found. Configure it in your .env");

        // Entity Framework DbContext for write operations (Commands)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Query DbContext for read operations (Dapper)
        services.AddScoped<QueryDbContext>(provider =>
            new QueryDbContext(connectionString));

        // HttpClient
        services.AddHttpClient();

        // Infrastructure specific services
        services.AddScoped<IChapterCheckerFactory, ChapterCheckerFactory>();

        // Background service
        services.AddHostedService<ChapterCheckingBackgroundService>();

        return services;
    }
}
