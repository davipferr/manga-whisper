using Microsoft.EntityFrameworkCore;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Repositories;
using MangaWhisper.Infrastructure.Data;

namespace MangaWhisper.Infrastructure.Repositories;

public class WorkerHeartbeatRepository : IWorkerHeartbeatRepository
{
    private readonly ApplicationDbContext _context;

    public WorkerHeartbeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkerHeartbeat?> GetAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WorkerHeartbeats
            .AsNoTracking()
            .FirstOrDefaultAsync(h => h.Id == WorkerHeartbeat.SingletonId, cancellationToken);
    }
}
