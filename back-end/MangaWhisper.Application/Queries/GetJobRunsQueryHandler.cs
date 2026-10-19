using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Mappers;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Queries;

public class GetJobRunsQueryHandler : IRequestHandler<GetJobRunsQuery, JobRunListResponseDto>
{
    private readonly IJobRunRepository _jobRunRepository;
    private readonly ILogger<GetJobRunsQueryHandler> _logger;

    public GetJobRunsQueryHandler(IJobRunRepository jobRunRepository, ILogger<GetJobRunsQueryHandler> logger)
    {
        _jobRunRepository = jobRunRepository;
        _logger = logger;
    }

    public async Task<JobRunListResponseDto> Handle(GetJobRunsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var runs = await _jobRunRepository.GetRecentAsync(request.Take, cancellationToken);

            return new JobRunListResponseDto
            {
                Runs = runs.Select(AdminJobMapper.ToDto).ToList(),
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving job runs");
            return new JobRunListResponseDto
            {
                Success = false,
                ErrorMessage = "Failed to retrieve job runs from database"
            };
        }
    }
}
