using Microsoft.EntityFrameworkCore;
using MangaWhisper.Common.Enums;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Repositories;
using MangaWhisper.Infrastructure.Data;

namespace MangaWhisper.Infrastructure.Repositories;

public class JobRunRepository : IJobRunRepository
{
    private readonly ApplicationDbContext _context;

    public JobRunRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<JobRun>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        return await _context.JobRuns
            .AsNoTracking()
            .Include(r => r.MangaChecker)
                .ThenInclude(mc => mc.Manga)
            .OrderByDescending(r => r.RequestedAt)
            .ThenByDescending(r => r.Id)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    // The worker executes one run at a time
    public async Task<JobRun?> GetRunningAsync(CancellationToken cancellationToken = default)
    {
        return await _context.JobRuns
            .AsNoTracking()
            .Include(r => r.MangaChecker)
                .ThenInclude(mc => mc.Manga)
            .Where(r => r.Status == JobRunStatus.Running)
            .OrderByDescending(r => r.StartedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasOpenRunForCheckerAsync(int mangaCheckerId, CancellationToken cancellationToken = default)
    {
        return await _context.JobRuns
            .AnyAsync(r => r.MangaCheckerId == mangaCheckerId
                && (r.Status == JobRunStatus.Pending || r.Status == JobRunStatus.Running), cancellationToken);
    }

    public async Task AddAsync(JobRun run, CancellationToken cancellationToken = default)
    {
        await _context.JobRuns.AddAsync(run, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
