using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Repositories;

public interface IWorkerHeartbeatRepository
{
    Task<WorkerHeartbeat?> GetAsync(CancellationToken cancellationToken = default);
}
