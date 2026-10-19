using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Entities;

/// <summary>
/// A recurring scraping job, created in the admin panel. The worker reads this table and executes the job
/// when <see cref="NextRunAt"/> is reached.
/// </summary>
public class ScrapingJob
{
    public int Id { get; set; }
    public int MangaCheckerId { get; set; }
    public ScrapingJobType Type { get; set; }
    public string CronExpression { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTime? NextRunAt { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual MangaChecker MangaChecker { get; set; } = null!;

    /// <param name="nextRunAt">Next occurrence of the cron expression, computed by the caller</param>
    public void Configure(ScrapingJobType type, string cronExpression, bool isEnabled, DateTime? nextRunAt)
    {
        Type = type;
        CronExpression = cronExpression;
        IsEnabled = isEnabled;
        // A disabled job has no next run; enabling it again schedules it from now on (no catch-up)
        NextRunAt = isEnabled ? nextRunAt : null;
    }
}
