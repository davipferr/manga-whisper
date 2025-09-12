using Microsoft.AspNetCore.Mvc;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Common.DTOs.Responses;
using MangaWhisper.Common.Enums;
using MangaWhisper.Application.Services;

namespace MangaWhisper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestMugiwaraController : ControllerBase
{
    private readonly ILogger<TestMugiwaraController> _logger;
    private readonly IChapterCheckingService _chapterCheckingService;

    public TestMugiwaraController(
        ILogger<TestMugiwaraController> logger,
        IChapterCheckingService chapterCheckingService)
    {
        _logger = logger;
        _chapterCheckingService = chapterCheckingService;
    }

    [HttpPost("setup-test-checkers")]
    public async Task<ActionResult<TestCheckerResponseDto>> SetupTestCheckers()
    {
        try
        {
            var onePieceManga = new Manga
            {
                Id = 1,
                Title = "One Piece",
                Status = MangaStatus.Ongoing,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create checker for Mugiwara Oficial
            var mugiwaraChecker = new MangaChecker
            {
                MangaId = onePieceManga.Id,
                SiteIdentifier = "mugiwara-oficial",
                CheckIntervalMinutes = 1, // Check every minute for testing
                IsActive = true,
                CheckerStatus = MangaCheckerStatus.Idle,
                CreatedAt = DateTime.UtcNow,
                LastKnownChapter = 1100,
                Manga = onePieceManga
            };

            await _chapterCheckingService.AddCheckerAsync(mugiwaraChecker);

            var response = new TestCheckerResponseDto
            {
                Success = true,
                Message = "Test checkers created successfully",
                Checkers = new List<CheckerInfoDto>
                {
                    new()
                    {
                        Id = mugiwaraChecker.Id,
                        SiteIdentifier = mugiwaraChecker.SiteIdentifier,
                        MangaTitle = onePieceManga.Title,
                        Status = mugiwaraChecker.CheckerStatus.ToString(),
                        IsActive = mugiwaraChecker.IsActive,
                        CheckIntervalMinutes = mugiwaraChecker.CheckIntervalMinutes
                    }
                }
            };

            _logger.LogInformation("Test checkers setup completed successfully");
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up test checkers");
            return StatusCode(500, new TestCheckerResponseDto
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            });
        }
    }

    [HttpGet("checkers-status")]
    public async Task<ActionResult<TestCheckerResponseDto>> GetCheckersStatus()
    {
        try
        {
            var checkers = await _chapterCheckingService.GetActiveCheckersAsync();

            var response = new TestCheckerResponseDto
            {
                Success = true,
                Message = $"Found {checkers.Count()} checkers",
                Checkers = checkers.Select(c => new CheckerInfoDto
                {
                    Id = c.Id,
                    SiteIdentifier = c.SiteIdentifier,
                    MangaTitle = c.Manga?.Title ?? "Unknown",
                    Status = c.CheckerStatus.ToString(),
                    IsActive = c.IsActive,
                    CheckIntervalMinutes = c.CheckIntervalMinutes
                }).ToList()
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting checkers status");
            return StatusCode(500, new TestCheckerResponseDto
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            });
        }
    }

    [HttpPost("trigger-manual-check")]
    public async Task<ActionResult<TestCheckerResponseDto>> TriggerManualCheck()
    {
        try
        {
            var checkers = await _chapterCheckingService.GetActiveCheckersAsync();
            
            if (!checkers.Any())
            {
                return BadRequest(new TestCheckerResponseDto
                {
                    Success = false,
                    Message = "No active checkers found. Use setup-test-checkers first."
                });
            }

            foreach (var checker in checkers)
            {
                var hasNewChapter = await _chapterCheckingService.HasNewChapterAsync(checker);

                if (hasNewChapter)
                {
                    _logger.LogDebug("New chapter detected for manga {MangaTitle}, extracting chapter information...",
                        checker.Manga?.Title ?? "Unknown");

                    // Step 2: If new chapter exists, extract the chapter information
                    var newChapter = await _chapterCheckingService.ExtractNewChapterInfoAsync(checker);

                    if (newChapter != null)
                    {
                        _logger.LogInformation(
                            "New chapter found for manga {MangaTitle}: Chapter {ChapterNumber} from site {SiteIdentifier}",
                            checker.Manga?.Title ?? "Unknown",
                            newChapter.Number,
                            checker.SiteIdentifier);

                        // TODO: Implement these features
                        // 1. Save the new chapter to database
                        // 2. Send notifications (WhatsApp, email, etc.)
                        // 3. Update the checker's LastKnownChapter
                    }
                    else
                    {
                        _logger.LogWarning("New chapter was detected but failed to extract chapter information for manga {MangaTitle} from site {SiteIdentifier}",
                            checker.Manga?.Title ?? "Unknown", checker.SiteIdentifier);
                    }
                }
                else
                {
                    _logger.LogDebug("No new chapter found for manga {MangaTitle} from site {SiteIdentifier}",
                        checker.Manga?.Title ?? "Unknown", checker.SiteIdentifier);
                }
            }

            return Ok(new TestCheckerResponseDto
            {
                Success = true,
                Message = $"Manual check triggered for {checkers.Count()} checkers"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error triggering manual check");
            return StatusCode(500, new TestCheckerResponseDto
            {
                Success = false,
                Message = $"Error: {ex.Message}"
            });
        }
    }
}
