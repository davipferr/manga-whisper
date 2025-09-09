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

    protected virtual bool RequiresSelenium => true;

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
            var expectedChapterNumber = subscription.GetExpectedNextChapter();
            var chapterUrl = BuildChapterUrl(subscription.MangaBaseUrl, expectedChapterNumber);

            // Check if this scraper requires Selenium
            if (!RequiresSelenium)
            {
                // Use HTTP-only check
                var httpResult = await CheckChapterExistsViaHttp(chapterUrl);
                return httpResult.Success && httpResult.ChapterExists;
            }

            return await CheckChapterExistsViaSelenium(chapterUrl);
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
