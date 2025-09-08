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

    [HttpGet("test-url/{*url}")]
    public async Task<ActionResult<TestCheckerResponseDto>> TestMugiwaraUrl(string url)
    {
        _logger.LogInformation("Testing Mugiwara URL: {Url}", url);

        if (string.IsNullOrWhiteSpace(url))
        {
            return BadRequest(new TestCheckerResponseDto
            {
                Success = false,
                ErrorMessage = "URL is required"
            });
        }

        // Decode the URL (in case it was URL encoded)
        url = Uri.UnescapeDataString(url);

        IWebDriver? webDriver = null;
        MugiwaraOficialChecker? checker = null;

        try
        {
            // Configure Chrome options for headless operation
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--window-size=1920,1080");
            chromeOptions.AddArgument("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

            // Create WebDriver instance
            webDriver = new ChromeDriver(chromeOptions);
            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Create checker instance
            var checkerLogger = _loggerFactory.CreateLogger<MugiwaraOficialChecker>();
            checker = new MugiwaraOficialChecker(webDriver, _httpClient, checkerLogger);

            var response = new TestCheckerResponseDto
            {
                SiteName = checker.GetSiteName(),
                Success = true
            };

            // Test URL validation
            response.IsUrlValid = checker.ValidateMangaUrl(url);
            
            if (!response.IsUrlValid)
            {
                response.ErrorMessage = "Invalid Mugiwara Oficial URL format";
                return Ok(response);
            }

            // Test manga info extraction
            try
            {
                var mangaInfo = await checker.ExtractMangaInfo(url);
                if (mangaInfo != null)
                {
                    response.MangaInfo = new MangaInfoResponseDto
                    {
                        Title = mangaInfo.Title,
                        CoverImageUrl = mangaInfo.CoverImageUrl,
                        Status = mangaInfo.Status,
                        LatestChapterNumber = mangaInfo.LatestChapterNumber,
                        BaseUrl = mangaInfo.BaseUrl
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting manga info");
                response.ErrorMessage = $"Error extracting manga info: {ex.Message}";
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing Mugiwara URL: {Url}", url);
            return StatusCode(500, new TestCheckerResponseDto
            {
                Success = false,
                ErrorMessage = $"Internal error: {ex.Message}"
            });
        }
        finally
        {
            // Cleanup resources
            try
            {
                checker?.Dispose();
                webDriver?.Quit();
                webDriver?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing WebDriver resources");
            }
        }
    }

    [HttpPost("test-chapter-check")]
    public async Task<ActionResult<TestCheckerResponseDto>> TestChapterCheck([FromBody] TestChapterCheckRequest request)
    {
        _logger.LogInformation("Testing chapter check for URL: {Url}, Chapter: {Chapter}", request.MangaUrl, request.ChapterNumber);

        if (string.IsNullOrWhiteSpace(request.MangaUrl) || request.ChapterNumber <= 0)
        {
            return BadRequest(new TestCheckerResponseDto
            {
                Success = false,
                ErrorMessage = "Valid URL and chapter number are required"
            });
        }

        IWebDriver? webDriver = null;
        MugiwaraOficialChecker? checker = null;

        try
        {
            // Configure Chrome options for headless operation
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--headless");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-dev-shm-usage");
            chromeOptions.AddArgument("--disable-gpu");
            chromeOptions.AddArgument("--window-size=1920,1080");

            webDriver = new ChromeDriver(chromeOptions);
            webDriver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
            webDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            var checkerLogger = _loggerFactory.CreateLogger<MugiwaraOficialChecker>();
            checker = new MugiwaraOficialChecker(webDriver, _httpClient, checkerLogger);

            // Create a mock subscription for testing
            var mockSubscription = new MangaSubscription
            {
                Id = 1,
                MangaId = 1,
                MangaBaseUrl = request.MangaUrl,
                LastKnownChapter = request.ChapterNumber - 1,
                Manga = new Manga { Id = 1, Title = "Test Manga" },
                Source = new MangaSource { Id = 1, Name = "Mugiwara Oficial" }
            };

            var checkResult = await checker.CheckForNewChapter(mockSubscription);

            var response = new TestCheckerResponseDto
            {
                SiteName = checker.GetSiteName(),
                Success = true,
                ChapterCheck = new ChapterCheckResponseDto
                {
                    Success = checkResult.Success,
                    ChapterExists = checkResult.ChapterExists,
                    ErrorMessage = checkResult.ErrorMessage,
                    RequiresSelenium = checkResult.RequiresSelenium,
                    IsAntiBotDetected = checkResult.IsAntiBotDetected,
                    NewChapter = checkResult.NewChapter != null ? new ChapterDto
                    {
                        Number = checkResult.NewChapter.Number,
                        Title = checkResult.NewChapter.Title,
                        Url = checkResult.NewChapter.Url
                    } : null
                }
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error testing chapter check");
            return StatusCode(500, new TestCheckerResponseDto
            {
                Success = false,
                ErrorMessage = $"Internal error: {ex.Message}"
            });
        }
        finally
        {
            try
            {
                checker?.Dispose();
                webDriver?.Quit();
                webDriver?.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing WebDriver resources");
            }
        }
    }

    [HttpGet("site-info")]
    public ActionResult<object> GetSiteInfo()
    {
        return Ok(new
        {
            SiteName = "Mugiwara Oficial",
            BaseUrl = "https://mugiwarasoficial.com",
            SupportedUrlPattern = "https://mugiwarasoficial.com/manga/{manga-slug}/",
            ExampleUrl = "https://mugiwarasoficial.com/manga/one-piece/"
        });
    }
}

public class TestChapterCheckRequest
{
    public string MangaUrl { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
}
