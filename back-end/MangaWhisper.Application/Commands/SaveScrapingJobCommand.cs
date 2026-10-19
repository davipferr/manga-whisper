using MediatR;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Application.Commands;

/// <summary>
/// Creates a scraping job (JobId null) or updates an existing one
/// </summary>
public record SaveScrapingJobCommand(int? JobId, int MangaCheckerId, ScrapingJobType Type, string CronExpression, bool IsEnabled)
    : IRequest<ScrapingJobResultDto>;
