using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaWhisper.Application.Commands;
using MangaWhisper.Application.Queries;
using MangaWhisper.Common.DTOs.Requests;
using MangaWhisper.Common.DTOs.Responses.Admin;

namespace MangaWhisper.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ADMIN")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AdminController> _logger;

    public AdminController(IMediator mediator, ILogger<AdminController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get every manga checker with its status and the number of chapters stored for its manga
    /// </summary>
    [HttpGet("checkers")]
    public async Task<ActionResult<AdminCheckerListResponseDto>> GetCheckers()
    {
        var result = await _mediator.Send(new GetAdminCheckersQuery());

        if (!result.Success)
        {
            _logger.LogWarning("Failed to retrieve checkers: {ErrorMessage}", result.ErrorMessage);
            return StatusCode(500, result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Pause/resume a checker or change the last chapter it knows about
    /// </summary>
    [HttpPatch("checkers/{id:int}")]
    public async Task<ActionResult<AdminCheckerResultDto>> UpdateChecker(int id, [FromBody] UpdateCheckerRequestDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _mediator.Send(new UpdateCheckerCommand(id, request.IsActive, request.LastKnownChapter));

        if (result.NotFound)
        {
            return NotFound(result);
        }

        if (!result.Success)
        {
            _logger.LogWarning("Failed to update checker {CheckerId}: {ErrorMessage}", id, result.ErrorMessage);
            return StatusCode(500, result);
        }

        return Ok(result);
    }
}
