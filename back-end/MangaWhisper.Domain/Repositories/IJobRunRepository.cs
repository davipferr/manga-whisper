using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Repositories;

public interface IJobRunRepository
{
    Task<List<JobRun>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
    Task<JobRun?> GetRunningAsync(CancellationToken cancellationToken = default);
    Task<bool> HasOpenRunForCheckerAsync(int mangaCheckerId, CancellationToken cancellationToken = default);
    Task AddAsync(JobRun run, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
