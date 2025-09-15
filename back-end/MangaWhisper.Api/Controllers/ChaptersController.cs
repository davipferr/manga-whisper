using MediatR;
using Microsoft.AspNetCore.Mvc;
using MangaWhisper.Application.Queries;
using MangaWhisper.Common.DTOs.Responses;

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
    /// Get all chapters from the database
    /// </summary>
    /// <returns>List of chapters</returns>
    [HttpGet]
    public async Task<ActionResult<ChaptersListResponseDto>> GetChapters()
    {
        try
        {
            var query = new GetChaptersQuery();
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
}