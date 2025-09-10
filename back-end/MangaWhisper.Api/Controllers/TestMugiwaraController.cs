using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using MangaWhisper.Domain.Services;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Common.DTOs.Responses;

namespace MangaWhisper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestMugiwaraController : ControllerBase
{
    private readonly ILogger<TestMugiwaraController> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly HttpClient _httpClient;

    public TestMugiwaraController(ILogger<TestMugiwaraController> logger, ILoggerFactory loggerFactory, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _loggerFactory = loggerFactory;
        _httpClient = httpClientFactory.CreateClient();
    }

    [HttpPost("test-chapter-check")]
    public async Task<ActionResult<TestCheckerResponseDto>> TestChapterCheck()
    {
        IWebDriver? webDriver;
        MugiwaraOficialChecker? checker = null;

        try
        {
            var chromeOptions = new ChromeOptions();
            // chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--window-size=1920,1080");

            webDriver = new ChromeDriver(chromeOptions);
            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var checkerLogger = _loggerFactory.CreateLogger<MugiwaraOficialChecker>();
            checker = new MugiwaraOficialChecker(webDriver, _httpClient, checkerLogger);

            // Create a mock subscription for testing
            var mockSubscription = new MangaChecker
            {
                Id = 1,
                MangaId = 1,
                LastKnownChapter = 0,
                Manga = new Manga { Id = 1, Title = "Test Manga" },
            };

            var hasNewChapter = await checker.GetNewChapter(mockSubscription);

            return Ok(hasNewChapter);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing chapter check");
            return StatusCode(500);
        }
        finally
        {
            checker?.Dispose();
        }
    }
}

public class TestChapterCheckRequest
{
    public string MangaUrl { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
}
