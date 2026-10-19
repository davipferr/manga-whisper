using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MangaWhisper.Application.Commands;
using MangaWhisper.Application.Queries;
using MangaWhisper.Common.DTOs.Requests;
using MangaWhisper.Common.DTOs.Responses.Admin;

namespace MangaWhisper.Api.Controllers;

/// <summary>
/// Scraping jobs, "Run now" requests and the worker status. The API only writes these to the database;
/// the background worker polls the tables and executes them.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "ADMIN")]
public class AdminJobsController : ControllerBase
{
    private const int MaxRunsPerPage = 100;

    private readonly IMediator _mediator;
    private readonly ILogger<AdminJobsController> _logger;

    public AdminJobsController(IMediator mediator, ILogger<AdminJobsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("jobs")]
    public async Task<ActionResult<ScrapingJobListResponseDto>> GetJobs()
    {
        var result = await _mediator.Send(new GetScrapingJobsQuery());

        if (!result.Success)
        {
            _logger.LogWarning("Failed to retrieve scraping jobs: {ErrorMessage}", result.ErrorMessage);
            return StatusCode(500, result);
        }

        return Ok(result);
    }

    [HttpPost("jobs")]
    public async Task<ActionResult<ScrapingJobResultDto>> CreateJob([FromBody] SaveScrapingJobRequestDto request)
    {
        var result = await _mediator.Send(new SaveScrapingJobCommand(
            null, request.MangaCheckerId, request.Type!.Value, request.CronExpression, request.IsEnabled));

        return ToJobActionResult(result, created: true);
    }

    [HttpPut("jobs/{id:int}")]
    public async Task<ActionResult<ScrapingJobResultDto>> UpdateJob(int id, [FromBody] SaveScrapingJobRequestDto request)
    {
        var result = await _mediator.Send(new SaveScrapingJobCommand(
            id, request.MangaCheckerId, request.Type!.Value, request.CronExpression, request.IsEnabled));

        return ToJobActionResult(result, created: false);
    }

    [HttpDelete("jobs/{id:int}")]
    public async Task<ActionResult<ScrapingJobResultDto>> DeleteJob(int id)
    {
        var result = await _mediator.Send(new DeleteScrapingJobCommand(id));

        return ToJobActionResult(result, created: false);
    }

    /// <summary>
    /// "Run now" for a checker. The worker starts it on its next poll (up to 30s)
    /// </summary>
    [HttpPost("checkers/{id:int}/runs")]
    public async Task<ActionResult<JobRunResultDto>> RequestRun(int id, [FromBody] RequestJobRunRequestDto request)
    {
        var result = await _mediator.Send(new RequestJobRunCommand(id, request.Type!.Value));

        if (result.NotFound)
        {
            return NotFound(result);
        }

        if (result.Conflict)
        {
            return Conflict(result);
        }

        if (!result.Success)
        {
            _logger.LogWarning("Failed to request a run for checker {CheckerId}: {ErrorMessage}", id, result.ErrorMessage);
            return StatusCode(500, result);
        }

        return StatusCode(201, result);
    }

    [HttpGet("runs")]
    public async Task<ActionResult<JobRunListResponseDto>> GetRuns([FromQuery] int take = 20)
    {
        var result = await _mediator.Send(new GetJobRunsQuery(Math.Clamp(take, 1, MaxRunsPerPage)));

        if (!result.Success)
        {
            _logger.LogWarning("Failed to retrieve job runs: {ErrorMessage}", result.ErrorMessage);
            return StatusCode(500, result);
        }

        return Ok(result);
    }

    [HttpGet("worker-status")]
    public async Task<ActionResult<WorkerStatusResponseDto>> GetWorkerStatus()
    {
        var result = await _mediator.Send(new GetWorkerStatusQuery());

        if (!result.Success)
        {
            _logger.LogWarning("Failed to retrieve the worker status: {ErrorMessage}", result.ErrorMessage);
            return StatusCode(500, result);
        }

        return Ok(result);
    }

    private ActionResult<ScrapingJobResultDto> ToJobActionResult(ScrapingJobResultDto result, bool created)
    {
        if (result.NotFound)
        {
            return NotFound(result);
        }

        if (result.Invalid)
        {
            return BadRequest(result);
        }

        if (!result.Success)
        {
            _logger.LogWarning("Scraping job request failed: {ErrorMessage}", result.ErrorMessage);
            return StatusCode(500, result);
        }

        return created ? StatusCode(201, result) : Ok(result);
    }
}
