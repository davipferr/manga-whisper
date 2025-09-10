using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Services;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Infrastructure.Services;

public class ChapterCheckingBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ChapterCheckingBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5);

    public ChapterCheckingBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ChapterCheckingBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Chapter Checking Background Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAllActiveSubscriptionsAsync(stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Chapter checking was cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking for new chapters");
            }

            try
            {
                await Task.Delay(_checkInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("Chapter Checking Background Service stopped");
    }

    private async Task CheckAllActiveSubscriptionsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var checkingService = scope.ServiceProvider.GetRequiredService<IChapterCheckingService>();

        var activeCheckers = await checkingService.GetActiveCheckersAsync();

        foreach (var checker in activeCheckers.Where(c => c.ShouldCheck()))
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                await ProcessCheckerAsync(checkingService, checker, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking manga {MangaTitle} with checker {CheckerId}",
                    checker.Manga?.Title ?? "Unknown", checker.Id);

                try
                {
                    await checkingService.UpdateCheckerStatusAsync(checker.Id, MangaCheckerStatus.Error);
                }
                catch (Exception updateEx)
                {
                    _logger.LogError(updateEx, "Failed to update checker status to Error for checker {CheckerId}", checker.Id);
                }
            }
        }
    }

    private async Task ProcessCheckerAsync(
        IChapterCheckingService checkingService,
        Domain.Entities.MangaChecker checker,
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Checking manga {MangaTitle} with site {SiteIdentifier}",
            checker.Manga?.Title ?? "Unknown", checker.SiteIdentifier);

        await checkingService.UpdateCheckerStatusAsync(checker.Id, MangaCheckerStatus.Checking);

        var newChapter = await checkingService.CheckForNewChapterAsync(checker);

        if (newChapter != null)
        {
            _logger.LogInformation(
                "New chapter found for manga {MangaTitle}: Chapter {ChapterNumber} from site {SiteIdentifier}",
                checker.Manga?.Title ?? "Unknown",
                newChapter.Number,
                checker.SiteIdentifier);

            // TODO: Implement these features
            // 1. Save the new chapter to database
            // 2. Send notifications (WhatsApp, email, etc.)
            // 3. Update the checker's LastKnownChapter
        }
        else
        {
            _logger.LogDebug("No new chapter found for manga {MangaTitle} from site {SiteIdentifier}",
                checker.Manga?.Title ?? "Unknown", checker.SiteIdentifier);
        }

        await checkingService.UpdateCheckerStatusAsync(checker.Id, MangaCheckerStatus.Idle);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Chapter Checking Background Service is stopping");
        await base.StopAsync(cancellationToken);
    }
}