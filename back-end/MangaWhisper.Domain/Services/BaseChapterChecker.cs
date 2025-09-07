using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Interfaces;

namespace MangaWhisper.Domain.Services;

public abstract class BaseChapterChecker : IChapterChecker, IDisposable
{
    protected readonly IWebDriver webDriver;
    protected readonly string siteName;
    protected readonly ILogger logger;
    protected readonly HttpClient httpClient;

    protected BaseChapterChecker(IWebDriver webDriver, HttpClient httpClient, ILogger logger)
    {
        this.webDriver = webDriver;
        this.httpClient = httpClient;
        this.logger = logger;
        siteName = GetSiteName();
    }

    public async Task<ChapterCheckResult> CheckForNewChapter(MangaSubscription subscription)
    {
        try
        {
            logger.LogInformation($"Checking for new chapter for manga {subscription.Manga.Title} from source {subscription.Source.Name}");

            var expectedChapterNumber = subscription.GetExpectedNextChapter();
            var chapterUrl = BuildChapterUrl(subscription.MangaBaseUrl, expectedChapterNumber);

            // Try HTTP request
            var httpResult = await CheckChapterExistsViaHttp(chapterUrl);
            
            // If HTTP request failed
            if (!httpResult.Success)
            {
                return ChapterCheckResult.Failure("HTTP request failed");
            }

            // If HTTP Is Successful but indicates Selenium is required
            if (httpResult.RequiresSelenium)
            {
                var chapter = await CheckChapterExistsViaSelenium(chapterUrl);
                if (chapter != null)
                {
                    chapter.MangaId = subscription.MangaId;
                }
                return ChapterCheckResult.SuccessWithChapter(chapter);
            }

            // If HTTP Is Successful but only need HTTP check
            if (httpResult.ChapterExists)
            {
                return ChapterCheckResult.SuccessHttpOnly(true);
            }

            return ChapterCheckResult.SuccessHttpOnly(false); // Chapter doesn't exist
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking for new chapter for subscription {SubscriptionId}", subscription.Id);
            return ChapterCheckResult.Failure($"Error checking for new chapter: {ex.Message}");
        }
    }

    public virtual bool ValidateMangaUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        try
        {
            var uri = new Uri(url);
            return !string.IsNullOrEmpty(GetMangaIdFromUrl(url));
        }
        catch
        {
            return false;
        }
    }

    public virtual async Task<MangaInfo?> ExtractMangaInfo(string url)
    {
        try
        {
            // Default implementation - can be overridden by specific checkers
            return await Task.FromResult<MangaInfo?>(null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting manga info from URL: {Url}", url);
            return null;
        }
    }

    public abstract string GetSiteName();

    // Abstract methods that must be implemented by concrete checkers
    protected abstract string BuildChapterUrl(string baseUrl, float chapterNumber);
    protected abstract Chapter? ExtractChapterFromHtml(string html, float chapterNumber);
    protected abstract string GetMangaIdFromUrl(string url);

    // Virtual methods that can be overridden
    protected virtual async Task<HttpCheckResult> CheckChapterExistsViaHttp(string url)
    {
        try
        {
            using var response = await httpClient.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                return HttpCheckResult.SuccessResult(true);
            }
            
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return HttpCheckResult.SuccessResult(false);
            }

            // Check if we might need Selenium (anti-bot protection, etc.)
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden ||
                response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                return HttpCheckResult.FailureResult("HTTP request blocked, may require Selenium", 
                    (int)response.StatusCode, true);
            }

            return HttpCheckResult.FailureResult($"HTTP error: {response.StatusCode}", 
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking chapter via HTTP: {Url}", url);
            return HttpCheckResult.FailureResult($"HTTP request failed: {ex.Message}", 0, true);
        }
    }

    protected virtual async Task<Chapter?> CheckChapterExistsViaSelenium(string url)
    {
        try
        {
            webDriver.Navigate().GoToUrl(url);
            
            // Handle potential anti-bot detection
            await HandleAntiBot();
            
            // Wait for page to load
            await Task.Delay(2000);
            
            var pageSource = webDriver.PageSource;
            
            // Check if page indicates chapter doesn't exist
            if (pageSource.Contains("404") || pageSource.Contains("not found") || 
                pageSource.Contains("página não encontrada"))
            {
                return null;
            }

            return ExtractChapterFromHtml(pageSource, 0); // Chapter number will be extracted from HTML
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking chapter via Selenium: {Url}", url);
            return null;
        }
    }

    protected virtual async Task HandleAntiBot()
    {
        // Default implementation - can be overridden for specific anti-bot handling
        await Task.Delay(1000);
    }

    public virtual void Dispose()
    {
        try
        {
            webDriver?.Dispose();
            httpClient?.Dispose();
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error disposing BaseChapterChecker resources");
        }
    }
}
