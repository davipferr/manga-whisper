using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Interfaces;

namespace MangaWhisper.Domain.Services;

public abstract class BaseChapterChecker : IChapterChecker
{
    protected readonly IWebDriver webDriver;
    protected readonly string siteName;
    protected readonly ILogger logger;
    protected readonly HttpClient httpClient;
    private bool _disposed = false;

    public abstract string SiteIdentifier { get; }
    protected virtual bool RequiresSelenium => false;
    protected abstract string chapterUrlPattern { get; }

    protected BaseChapterChecker(IWebDriver webDriver, HttpClient httpClient, ILogger logger)
    {
        this.webDriver = webDriver;
        this.httpClient = httpClient;
        this.logger = logger;
        siteName = GetSiteName();
    }

    protected abstract string GetSiteName();
    protected abstract Chapter ExtractNewChapterInfo();
    protected abstract string BuildChapterUrl(int chapterNumber);
    protected abstract bool CheckChapterExistsViaSeleniumRules(string pageSource);    

    public async Task<Chapter?> GetNewChapter(MangaChecker subscription)
    {
        try
        {
            logger.LogInformation("Checking for new chapter for manga {MangaTitle} from {SiteName}",
                subscription.Manga?.Title ?? "Unknown", GetSiteName());

            var nextChapter = subscription.GetExpectedNextChapter();
            var chapterUrl = BuildChapterUrl(nextChapter);

            if (string.IsNullOrEmpty(chapterUrl))
            {
                logger.LogWarning("Could not build chapter URL for chapter {ChapterNumber}", nextChapter);
                return null;
            }

            logger.LogDebug("Checking URL: {ChapterUrl}", chapterUrl);

            // For now, simulate a check that sometimes finds a new chapter
            await Task.Delay(1000); // Simulate web scraping delay

            // Simulate finding a new chapter 30% of the time for testing
            var random = new Random();
            if (random.NextDouble() < 0.3)
            {
                var newChapter = new Chapter
                {
                    Number = nextChapter,
                    Title = $"Capítulo {nextChapter}",
                    Url = chapterUrl,
                    PublishedDate = DateTime.UtcNow,
                    MangaId = subscription.MangaId
                };

                logger.LogInformation("New chapter found: {ChapterTitle}", newChapter.Title);
                return newChapter;
            }

            logger.LogDebug("No new chapter found for {SiteName}", GetSiteName());
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking for new chapter from {SiteName}", GetSiteName());
            return null;
        }
    }

    private async Task<bool> HasNewChapter(MangaChecker subscription)
    {
        try
        {
            var expectedChapterNumber = subscription.GetExpectedNextChapter();
            var chapterUrl = BuildChapterUrl(expectedChapterNumber);

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

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                webDriver?.Dispose();
                httpClient?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
