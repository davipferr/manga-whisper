using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Commands;

public class DeleteScrapingJobCommandHandler : IRequestHandler<DeleteScrapingJobCommand, ScrapingJobResultDto>
{
    private readonly IScrapingJobRepository _scrapingJobRepository;
    private readonly ILogger<DeleteScrapingJobCommandHandler> _logger;

    public DeleteScrapingJobCommandHandler(IScrapingJobRepository scrapingJobRepository, ILogger<DeleteScrapingJobCommandHandler> logger)
    {
        _scrapingJobRepository = scrapingJobRepository;
        _logger = logger;
    }

    public async Task<ScrapingJobResultDto> Handle(DeleteScrapingJobCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var job = await _scrapingJobRepository.GetByIdAsync(request.JobId, cancellationToken);
            if (job == null)
            {
                return new ScrapingJobResultDto
                {
                    Success = false,
                    NotFound = true,
                    ErrorMessage = $"Job {request.JobId} does not exist"
                };
            }

            // Its past runs stay in the history (JobRuns.ScrapingJobId becomes NULL)
            _scrapingJobRepository.Remove(job);
            await _scrapingJobRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Scraping job {JobId} deleted", request.JobId);

            return new ScrapingJobResultDto { Success = true };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting scraping job {JobId}", request.JobId);
            return new ScrapingJobResultDto
            {
                Success = false,
                ErrorMessage = "Failed to delete the job"
            };
        }
    }
}
