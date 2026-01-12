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
}
