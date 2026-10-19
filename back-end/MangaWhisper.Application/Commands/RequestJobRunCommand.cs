using MediatR;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Application.Commands;

/// <summary>
/// "Run now": queues a run the worker executes on its next poll
/// </summary>
public record RequestJobRunCommand(int CheckerId, ScrapingJobType Type) : IRequest<JobRunResultDto>;
