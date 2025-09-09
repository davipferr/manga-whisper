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

    protected abstract string GetSiteName();
    protected abstract Chapter ExtractNewChapterInfo();
    protected abstract string BuildChapterUrl(string baseUrl, int chapterNumber);
    protected abstract bool CheckChapterExistsViaSeleniumRules(string pageSource);

    public async Task<Chapter?> GetNewChapter(MangaSubscription subscription)
    {
        try
        {
            var hasNewChapter = await HasNewChapter(subscription);

            if (hasNewChapter)
            {
                return ExtractNewChapterInfo();
            }

            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting new chapter info from page source");
            return null;
        }
    }

    private async Task<bool> HasNewChapter(MangaSubscription subscription)
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
                return false;
            }

            // If HTTP Is Successful but indicates Selenium is required
            if (httpResult.RequiresSelenium)
            {
                return await CheckChapterExistsViaSelenium(chapterUrl);
            }

            // If HTTP Is Successful but only need HTTP check
            if (httpResult.ChapterExists)
            {
                return true;
            }

            // If HTTP Is Successful but chapter doesn't exist    
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking for new chapter for subscription {SubscriptionId}", subscription.Id);
            return false;
        }
    }

    private async Task<HttpCheckResult> CheckChapterExistsViaHttp(string url)
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

            return HttpCheckResult.FailureResult($"HTTP error: {response.StatusCode}",
                (int)response.StatusCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking chapter via HTTP: {Url}", url);
            return HttpCheckResult.FailureResult($"HTTP request failed: {ex.Message}", 0);
        }
    }

    private async Task<bool> CheckChapterExistsViaSelenium(string url)
    {
        try
        {
            webDriver.Navigate().GoToUrl(url);

            await Task.Delay(2000);

            var pageSource = webDriver.PageSource;

            if (pageSource.Contains("404") || pageSource.Contains("not found") ||
                pageSource.Contains("página não encontrada"))
            {
                return false;
            }

            return CheckChapterExistsViaSeleniumRules(pageSource);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking chapter via Selenium: {Url}", url);
            return false;
        }
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
