using MediatR;
using Microsoft.AspNetCore.Mvc;
using MangaWhisper.Application.Commands;
using MangaWhisper.Application.Queries;
using MangaWhisper.Common.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;

namespace MangaWhisper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChaptersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ChaptersController> _logger;

    public ChaptersController(IMediator mediator, ILogger<ChaptersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    
    /// <summary>
    /// Get paginated chapters from the database
    /// </summary>
    /// <param name="page">The page number (default is 1)</param>
    /// <param name="pageSize">The number of items per page (default is 5)</param>
    /// <returns>Paginated list of chapters</returns>
    [HttpGet]
    public async Task<ActionResult<ChaptersListResponseDto>> GetChapters(int page = 1, int pageSize = 5)
    {
        try
        {
            var query = new GetChaptersQuery
            {
                Page = page,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to retrieve chapters: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving chapters");
            return StatusCode(500, new ChaptersListResponseDto
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred"
            });
        }
    }

    /// <summary>
    /// Trigger a manual chapter check
    /// </summary>
    /// <returns>Result of the manual chapter check</returns>
    /// <remarks>
    /// This endpoint is restricted to users with the ADMIN role.
    /// </remarks>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("check-now")]
    public async Task<ActionResult<ManualCheckResponseDto>> TriggerManualCheck()
    {
        try
        {
            var command = new TriggerManualChapterCheckCommand();
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                _logger.LogWarning("Manual chapter check failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during manual chapter check");
            return StatusCode(500, new ManualCheckResponseDto
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred"
            });
        }
    }

    /// <summary>
    /// Trigger processing all available chapters for a specific checker
    /// </summary>
    /// <param name="checkerId">The ID of the checker</param>
    /// <returns>Result of the process all available chapters</returns>
    /// <remarks>
    /// This endpoint is restricted to users with the ADMIN role.
    /// </remarks>
    [Authorize(Roles = "ADMIN")]
    [HttpPost("process-all-available/{checkerId}")]
    public async Task<ActionResult<ManualCheckResponseDto>> TriggerProcessAllAvailableChapters(int checkerId)
    {
        try
        {
            var command = new TriggerManualProcessAllAvailableChaptersCommand(checkerId);
            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                _logger.LogWarning("Process all chapters failed: {ErrorMessage}", result.ErrorMessage);
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred during process all chapters");
            return StatusCode(500, new ManualCheckResponseDto
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred"
            });
        }
    }
}