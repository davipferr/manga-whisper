using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Services;

public class MugiwaraOficialChecker : BaseChapterChecker
{
    public override string SiteIdentifier => "mugiwara-oficial";
    protected override string chapterUrlPattern => "https://mugiwarasoficial.com/manga/one-piece/capitulo-{1}/";

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

    protected override bool CheckChapterExistsViaSeleniumRules(string pageSource)
    {
        try
        {
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if chapter exists from page source");
            return false;
        }
    }
}
