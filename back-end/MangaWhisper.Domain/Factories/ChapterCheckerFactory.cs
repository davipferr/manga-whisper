using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using MangaWhisper.Domain.Interfaces;
using MangaWhisper.Domain.Services;

namespace MangaWhisper.Domain.Factories;

public class ChapterCheckerFactory : IChapterCheckerFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IHttpClientFactory _httpClientFactory;

    public ChapterCheckerFactory(
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory,
        IHttpClientFactory httpClientFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
        _httpClientFactory = httpClientFactory;
    }

    public IChapterChecker CreateChecker(string siteIdentifier)
    {
        var webDriver = CreateWebDriver();
        var httpClient = _httpClientFactory.CreateClient();

        return siteIdentifier.ToLowerInvariant() switch
        {
            "mugiwara-oficial" => new MugiwaraOficialChecker(
                webDriver,
                httpClient,
                _loggerFactory.CreateLogger<MugiwaraOficialChecker>()),

            _ => throw new NotSupportedException($"Site identifier '{siteIdentifier}' is not supported")
        };
    }

    public IEnumerable<string> GetAvailableSites()
    {
        return new[]
        {
            "mugiwara-oficial",
            "mangadex"
        };
    }

    private IWebDriver CreateWebDriver()
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless");
        chromeOptions.AddArgument("--disable-gpu");
        chromeOptions.AddArgument("--window-size=1920,1080");
        chromeOptions.AddArgument("--no-sandbox");
        chromeOptions.AddArgument("--disable-dev-shm-usage");

        return new ChromeDriver(chromeOptions);
    }
}