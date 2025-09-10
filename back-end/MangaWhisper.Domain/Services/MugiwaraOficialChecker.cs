using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Services;

public class MugiwaraOficialChecker : BaseChapterChecker
{
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
            logger.LogError(
                ex,
                $"Error building chapter URL for chapter: {chapterNumber}, Checker: {GetSiteName()}"
            );

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

    private int? ExtractLatestChapterNumber(string pageSource)
    {
        try
        {
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting latest chapter number from page source");
            return null;
        }
    }

    private string ExtractChapterTitle(string pageSource)
    {
        try
        {
            return "Capítulo " + ExtractLatestChapterNumber(pageSource)?.ToString() ?? string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting chapter title from page source");
            return $"Capítulo {ExtractLatestChapterNumber(pageSource)?.ToString() ?? string.Empty}";
        }
    }
}
