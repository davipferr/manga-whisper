using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Mappers;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Queries;

public class GetScrapingJobsQueryHandler : IRequestHandler<GetScrapingJobsQuery, ScrapingJobListResponseDto>
{
    private readonly IScrapingJobRepository _scrapingJobRepository;
    private readonly ILogger<GetScrapingJobsQueryHandler> _logger;

    public GetScrapingJobsQueryHandler(IScrapingJobRepository scrapingJobRepository, ILogger<GetScrapingJobsQueryHandler> logger)
    {
        _scrapingJobRepository = scrapingJobRepository;
        _logger = logger;
    }

    public async Task<ScrapingJobListResponseDto> Handle(GetScrapingJobsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var jobs = await _scrapingJobRepository.GetAllAsync(cancellationToken);

            return new ScrapingJobListResponseDto
            {
                Jobs = jobs.Select(AdminJobMapper.ToDto).ToList(),
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving scraping jobs");
            return new ScrapingJobListResponseDto
            {
                Success = false,
                ErrorMessage = "Failed to retrieve scraping jobs from database"
            };
        }
    }
}
