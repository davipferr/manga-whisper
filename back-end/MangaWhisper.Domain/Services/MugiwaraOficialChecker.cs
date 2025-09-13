using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Services;

public class MugiwaraOficialChecker : BaseChapterChecker
{
    public override string SiteIdentifier => "mugiwara-oficial";
    protected override string chapterUrlPattern => "https://mugiwarasoficial.com/manga/one-piece/capitulo-{1}/";
    protected override bool RequiresSelenium => true;

    public MugiwaraOficialChecker(IWebDriver webDriver, HttpClient httpClient, ILogger<MugiwaraOficialChecker> logger)
        : base(webDriver, httpClient, logger)
    {
    }

    protected override string GetSiteName()
    {
        return "Mugiwara Oficial";
    }

    protected override string BuildChapterUrl(int chapterNumber)
    {
        try
        {
            return chapterUrlPattern.Replace("{1}", chapterNumber.ToString());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error building chapter URL for chapter: {ChapterNumber}, Checker: {SiteName}",
                chapterNumber, GetSiteName());
            return string.Empty;
        }
    }

    protected override Chapter ExtractNewChapterInfo()
    {
        try
        {
            return new Chapter();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if chapter exists from page source");
            return new Chapter();
        }
    }

    protected override async Task<bool> CheckChapterExistsViaSeleniumRules()
    {
        try
        {
            logger.LogInformation("Starting chapter existence check for {SiteName} using Selenium", GetSiteName());

            var mainColElement = webDriver.FindElement(By.CssSelector("div.content-area div.container div.row div.main-col"));
            var chapterHeadingElement = mainColElement.FindElement(By.CssSelector("h1#chapter-heading"));

            logger.LogInformation("Chapter existence check completed for {SiteName}. Chapter exists: {ChapterExists}",
                GetSiteName(), true);

            return await Task.FromResult(true);
        }
        catch (NoSuchElementException ex)
        {
            logger.LogInformation("Required elements not found on page for {SiteName}. Chapter does not exist. Details: '{ErrorDetails}'",
                GetSiteName(), ex.Message);
            return false;
        }
        catch (WebDriverTimeoutException ex)
        {
            logger.LogError(ex, "Timeout occurred while checking chapter existence for {SiteName}", GetSiteName());
            return false;
        }
        catch (WebDriverException ex)
        {
            logger.LogError(ex, "WebDriver error occurred while checking chapter existence for {SiteName}", 
                GetSiteName());
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error checking if chapter exists from page source for {SiteName}", 
                GetSiteName());
            return false;
        }
    }
}
