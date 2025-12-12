using MediatR;
using Microsoft.AspNetCore.Mvc;
using MangaWhisper.Application.Commands;
using MangaWhisper.Application.Queries;
using MangaWhisper.Common.DTOs.Responses.MangaChecker;
using Microsoft.AspNetCore.Authorization;

namespace MangaWhisper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MangaCheckersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<MangaCheckersController> _logger;

    public MangaCheckersController(IMediator mediator, ILogger<MangaCheckersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("get-checker-by-manga-title/{mangaTitle}")]
    public async Task<ActionResult<MangaCheckerListResponseDto>> GetCheckerByMangaTitle(string mangaTitle)
    {
        try
        {
            var query = new GetMangaCheckerByMangaTitleQuery(mangaTitle);
            var result = await _mediator.Send(query);

            if (!result.Success)
            {
                _logger.LogWarning("Failed to retrieve manga checker for MangaTitle {MangaTitle}: {ErrorMessage}", mangaTitle, result.ErrorMessage);
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error occurred while retrieving manga checker for MangaTitle {MangaTitle}", mangaTitle);
            return StatusCode(500, new MangaCheckerListResponseDto
            {
                Success = false,
                ErrorMessage = "An unexpected error occurred"
            });
        }
    }

}