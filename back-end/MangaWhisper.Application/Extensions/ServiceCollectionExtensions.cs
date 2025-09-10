using Microsoft.Extensions.DependencyInjection;
using MangaWhisper.Application.Services;

namespace MangaWhisper.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application layer services only
        services.AddScoped<IChapterCheckingService, ChapterCheckingService>();

        // MediatR for CQRS pattern
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

        return services;
    }
}