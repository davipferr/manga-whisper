using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Mappers;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Queries;

public class GetWorkerStatusQueryHandler : IRequestHandler<GetWorkerStatusQuery, WorkerStatusResponseDto>
{
    // The worker beats every 30s; missing a few beats means it is down
    private static readonly TimeSpan OfflineAfter = TimeSpan.FromSeconds(90);

    private readonly IWorkerHeartbeatRepository _workerHeartbeatRepository;
    private readonly IJobRunRepository _jobRunRepository;
    private readonly ILogger<GetWorkerStatusQueryHandler> _logger;

    public GetWorkerStatusQueryHandler(
        IWorkerHeartbeatRepository workerHeartbeatRepository,
        IJobRunRepository jobRunRepository,
        ILogger<GetWorkerStatusQueryHandler> logger)
    {
        _workerHeartbeatRepository = workerHeartbeatRepository;
        _jobRunRepository = jobRunRepository;
        _logger = logger;
    }

    public async Task<WorkerStatusResponseDto> Handle(GetWorkerStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var heartbeat = await _workerHeartbeatRepository.GetAsync(cancellationToken);
            var isOnline = heartbeat != null && heartbeat.StoppedAt == null && DateTime.UtcNow - heartbeat.LastSeenAt < OfflineAfter;

            // A Running run left behind by a crashed worker is not shown; the worker fails it when it starts again
            var currentRun = isOnline ? await _jobRunRepository.GetRunningAsync(cancellationToken) : null;

            return new WorkerStatusResponseDto
            {
                IsOnline = isOnline,
                LastSeenAt = heartbeat?.LastSeenAt,
                StartedAt = heartbeat?.StartedAt,
                CurrentRun = currentRun != null ? AdminJobMapper.ToDto(currentRun) : null,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving the worker status");
            return new WorkerStatusResponseDto
            {
                Success = false,
                ErrorMessage = "Failed to retrieve the worker status"
            };
        }
    }
}
