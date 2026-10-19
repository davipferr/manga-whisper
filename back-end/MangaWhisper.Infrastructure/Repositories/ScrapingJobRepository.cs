using Microsoft.EntityFrameworkCore;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Repositories;
using MangaWhisper.Infrastructure.Data;

namespace MangaWhisper.Infrastructure.Repositories;

public class ScrapingJobRepository : IScrapingJobRepository
{
    private readonly ApplicationDbContext _context;

    public ScrapingJobRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<ScrapingJob>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ScrapingJobs
            .AsNoTracking()
            .Include(j => j.MangaChecker)
                .ThenInclude(mc => mc.Manga)
            .OrderBy(j => j.MangaChecker.Manga.Title)
            .ThenBy(j => j.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<ScrapingJob?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ScrapingJobs
            .Include(j => j.MangaChecker)
                .ThenInclude(mc => mc.Manga)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task AddAsync(ScrapingJob job, CancellationToken cancellationToken = default)
    {
        await _context.ScrapingJobs.AddAsync(job, cancellationToken);
    }

    public void Remove(ScrapingJob job)
    {
        _context.ScrapingJobs.Remove(job);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
