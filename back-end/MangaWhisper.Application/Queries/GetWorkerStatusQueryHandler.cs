using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Queries;

public class GetWorkerStatusQueryHandler : IRequestHandler<GetWorkerStatusQuery, WorkerStatusResponseDto>
{
    // The worker beats every 30s; missing a few beats means it is down
    private static readonly TimeSpan OfflineAfter = TimeSpan.FromSeconds(90);

    private readonly IWorkerHeartbeatRepository _workerHeartbeatRepository;
    private readonly ILogger<GetWorkerStatusQueryHandler> _logger;

    public GetWorkerStatusQueryHandler(IWorkerHeartbeatRepository workerHeartbeatRepository, ILogger<GetWorkerStatusQueryHandler> logger)
    {
        _workerHeartbeatRepository = workerHeartbeatRepository;
        _logger = logger;
    }

    public async Task<WorkerStatusResponseDto> Handle(GetWorkerStatusQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var heartbeat = await _workerHeartbeatRepository.GetAsync(cancellationToken);

            return new WorkerStatusResponseDto
            {
                IsOnline = heartbeat != null && DateTime.UtcNow - heartbeat.LastSeenAt < OfflineAfter,
                LastSeenAt = heartbeat?.LastSeenAt,
                StartedAt = heartbeat?.StartedAt,
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
