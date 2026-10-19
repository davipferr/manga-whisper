using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Application.Mappers;

public static class AdminJobMapper
{
    public static ScrapingJobResponseDto ToDto(ScrapingJob job)
    {
        return new ScrapingJobResponseDto
        {
            Id = job.Id,
            MangaCheckerId = job.MangaCheckerId,
            MangaTitle = job.MangaChecker?.Manga?.Title ?? string.Empty,
            Type = job.Type.ToString(),
            CronExpression = job.CronExpression,
            TimeZone = job.TimeZone,
            IsEnabled = job.IsEnabled,
            NextRunAt = job.NextRunAt,
            LastRunAt = job.LastRunAt,
            CreatedAt = job.CreatedAt
        };
    }

    public static JobRunResponseDto ToDto(JobRun run)
    {
        return new JobRunResponseDto
        {
            Id = run.Id,
            MangaCheckerId = run.MangaCheckerId,
            MangaTitle = run.MangaChecker?.Manga?.Title ?? string.Empty,
            ScrapingJobId = run.ScrapingJobId,
            Type = run.Type.ToString(),
            Status = run.Status.ToString(),
            RequestedAt = run.RequestedAt,
            StartedAt = run.StartedAt,
            FinishedAt = run.FinishedAt,
            ChaptersFound = run.ChaptersFound,
            ResultMessage = run.ResultMessage,
            ErrorMessage = run.ErrorMessage
        };
    }
}
