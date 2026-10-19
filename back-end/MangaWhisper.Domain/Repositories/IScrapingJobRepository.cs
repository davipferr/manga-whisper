using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Repositories;

public interface IScrapingJobRepository
{
    Task<List<ScrapingJob>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ScrapingJob?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddAsync(ScrapingJob job, CancellationToken cancellationToken = default);
    void Remove(ScrapingJob job);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
