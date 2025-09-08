using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System.Text.RegularExpressions;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Services;

public class MugiwaraOficialChecker : BaseChapterChecker
{
    private readonly string chapterUrlPattern = "https://mugiwarasoficial.com/manga/{0}/capitulo-{1}/";

    public MugiwaraOficialChecker(IWebDriver webDriver, HttpClient httpClient, ILogger<MugiwaraOficialChecker> logger)
        : base(webDriver, httpClient, logger)
    {
    }

    public override string GetSiteName()
    {
        return "Mugiwara Oficial";
    }

    protected override string BuildChapterUrl(string baseUrl, int chapterNumber)
    {
        try
        {
            var mangaSlug = GetMangaSlugFromUrl(baseUrl);
            if (string.IsNullOrEmpty(mangaSlug))
            {
                logger.LogWarning("Could not extract manga slug from URL: {BaseUrl}", baseUrl);
                return string.Empty;
            }

            return $"https://mugiwarasoficial.com/manga/{mangaSlug}/capitulo-{chapterNumber}/";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error building chapter URL for manga: {BaseUrl}, chapter: {ChapterNumber}", baseUrl, chapterNumber);
            return string.Empty;
        }
    }

    protected override string GetMangaIdFromUrl(string url)
    {
        try
        {
            var regex = new Regex(@"https://mugiwarasoficial\.com/manga/([^/]+)", RegexOptions.IgnoreCase);
            var match = regex.Match(url);
            
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            logger.LogWarning("Could not extract manga slug from Mugiwara Oficial URL: {Url}", url);
            return string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting manga ID from URL: {Url}", url);
            return string.Empty;
        }
    }

    private string GetMangaSlugFromUrl(string url)
    {
        return GetMangaIdFromUrl(url);
    }

    protected override Chapter? CheckIfChapterExistsRules(string pageSource)
    {
        try
        {
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking if chapter exists from page source");
            return null;
        }
    }

    public override async Task<MangaInfo?> ExtractMangaInfo(string url)
    {
        try
        {
            var mangaSlug = GetMangaSlugFromUrl(url);
            if (string.IsNullOrEmpty(mangaSlug))
            {
                logger.LogWarning("Could not extract manga slug from URL: {Url}", url);
                return null;
            }

            // Navigate to the manga main page
            var mangaBaseUrl = $"https://mugiwarasoficial.com/manga/{mangaSlug}/";
            
            return await ExtractMagaInfoViaSelenium(mangaBaseUrl);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting manga info from URL: {Url}", url);
            return null;
        }
    }

    private async Task<MangaInfo?> ExtractMagaInfoViaSelenium(string url)
    {
        try
        {
            webDriver.Navigate().GoToUrl(url);
            await Task.Delay(3000);

            var pageSource = webDriver.PageSource;
            
            // Extract cover image
            var coverImageUrl = ExtractChapterCoverImage(pageSource);

            // Extract latest chapter number
            var latestChapterNumber = ExtractLatestChapterNumber(pageSource);

            return new MangaInfo
            {
                CoverImageUrl = coverImageUrl,
                LatestChapterNumber = latestChapterNumber,
                BaseUrl = url
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting manga info via Selenium from URL: {Url}", url);
            return null;
        }
    }

    private string ExtractChapterCoverImage(string pageSource)
    {
        try
        {
            var patterns = new[]
            {
                @"<img[^>]*class=""[^""]*cover[^""]*""[^>]*src=""([^""]+)""",
                @"<img[^>]*class=""[^""]*manga-cover[^""]*""[^>]*src=""([^""]+)""",
                @"<meta[^>]*property=""og:image""[^>]*content=""([^""]+)""",
                @"<div[^>]*class=""[^""]*cover[^""]*""[^>]*>.*?<img[^>]*src=""([^""]+)""",
                @"<img[^>]*alt=""[^""]*cover[^""]*""[^>]*src=""([^""]+)"""
            };

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(pageSource, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                if (match.Success)
                {
                    var imageUrl = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Make sure it's an absolute URL
                        if (imageUrl.StartsWith("//"))
                        {
                            imageUrl = "https:" + imageUrl;
                        }
                        else if (imageUrl.StartsWith("/"))
                        {
                            imageUrl = "https://mugiwarasoficial.com" + imageUrl;
                        }
                        
                        return imageUrl;
                    }
                }
            }

            return string.Empty;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting cover image from page source");
            return string.Empty;
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

    private string ExtractChapterTitle(string pageSource, int chapterNumber)
    {
        try
        {
            return $"Não foi possivel encontrar o Título do Capítulo {chapterNumber}";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extracting chapter title from page source");
            return $"Capítulo {chapterNumber}";
        }
    }
}
