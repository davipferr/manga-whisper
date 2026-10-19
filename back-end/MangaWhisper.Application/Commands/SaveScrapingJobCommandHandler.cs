using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Mappers;
using MangaWhisper.Application.Scheduling;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Commands;

public class SaveScrapingJobCommandHandler : IRequestHandler<SaveScrapingJobCommand, ScrapingJobResultDto>
{
    private readonly IScrapingJobRepository _scrapingJobRepository;
    private readonly IMangaCheckerRepository _mangaCheckerRepository;
    private readonly ILogger<SaveScrapingJobCommandHandler> _logger;

    public SaveScrapingJobCommandHandler(
        IScrapingJobRepository scrapingJobRepository,
        IMangaCheckerRepository mangaCheckerRepository,
        ILogger<SaveScrapingJobCommandHandler> logger)
    {
        _scrapingJobRepository = scrapingJobRepository;
        _mangaCheckerRepository = mangaCheckerRepository;
        _logger = logger;
    }

    public async Task<ScrapingJobResultDto> Handle(SaveScrapingJobCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var cronExpression = request.CronExpression.Trim();

            if (!JobSchedule.TryGetNextOccurrence(cronExpression, DateTime.UtcNow, out var nextRunAt, out var cronError))
            {
                return new ScrapingJobResultDto { Success = false, Invalid = true, ErrorMessage = cronError };
            }

            var checker = await _mangaCheckerRepository.GetByIdAsync(request.MangaCheckerId);
            if (checker == null)
            {
                return new ScrapingJobResultDto
                {
                    Success = false,
                    Invalid = true,
                    ErrorMessage = $"Checker {request.MangaCheckerId} does not exist"
                };
            }

            ScrapingJob? job;

            if (request.JobId.HasValue)
            {
                job = await _scrapingJobRepository.GetByIdAsync(request.JobId.Value, cancellationToken);
                if (job == null)
                {
                    return new ScrapingJobResultDto
                    {
                        Success = false,
                        NotFound = true,
                        ErrorMessage = $"Job {request.JobId} does not exist"
                    };
                }
            }
            else
            {
                job = new ScrapingJob
                {
                    TimeZone = JobSchedule.DefaultTimeZone,
                    CreatedAt = DateTime.UtcNow
                };
                await _scrapingJobRepository.AddAsync(job, cancellationToken);
            }

            job.MangaCheckerId = checker.Id;
            job.MangaChecker = checker;
            job.Configure(request.Type, cronExpression, request.IsEnabled, nextRunAt);

            await _scrapingJobRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Scraping job {JobId} saved: {Type} '{Cron}' for checker {CheckerId}, enabled={IsEnabled}, next run {NextRunAt:u}",
                job.Id, job.Type, job.CronExpression, job.MangaCheckerId, job.IsEnabled, job.NextRunAt);

            return new ScrapingJobResultDto
            {
                Job = AdminJobMapper.ToDto(job),
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving scraping job {JobId}", request.JobId);
            return new ScrapingJobResultDto
            {
                Success = false,
                ErrorMessage = "Failed to save the job"
            };
        }
    }
}
