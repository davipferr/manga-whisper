using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Entities;

/// <summary>
/// One execution of a scraping job. The API only creates Pending runs ("Run now");
/// the worker picks them up and fills the rest.
/// </summary>
public class JobRun
{
    public int Id { get; set; }
    public int MangaCheckerId { get; set; }
    public int? ScrapingJobId { get; set; }
    public ScrapingJobType Type { get; set; }
    public JobRunStatus Status { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int ChaptersFound { get; set; }
    public string? ResultMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public virtual MangaChecker MangaChecker { get; set; } = null!;
    public virtual ScrapingJob? ScrapingJob { get; set; }

    public static JobRun RequestNow(int mangaCheckerId, ScrapingJobType type)
    {
        return new JobRun
        {
            MangaCheckerId = mangaCheckerId,
            Type = type,
            Status = JobRunStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };
    }
}
