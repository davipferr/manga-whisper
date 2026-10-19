using Cronos;

namespace MangaWhisper.Application.Scheduling;

/// <summary>
/// Cron rules for scraping jobs. The worker uses the same library (Cronos) to move a job to its next run,
/// so both projects agree on when a job runs.
/// </summary>
public static class JobSchedule
{
    // Every job is evaluated in BRT
    public const string DefaultTimeZone = "America/Sao_Paulo";

    /// <summary>
    /// Validates a 5-field cron expression and returns its next occurrence after <paramref name="fromUtc"/>
    /// </summary>
    public static bool TryGetNextOccurrence(string cronExpression, DateTime fromUtc, out DateTime? nextRunAt, out string? error)
    {
        nextRunAt = null;
        error = null;

        CronExpression expression;
        try
        {
            expression = CronExpression.Parse(cronExpression.Trim());
        }
        catch (CronFormatException ex)
        {
            error = $"Invalid cron expression: {ex.Message}";
            return false;
        }

        nextRunAt = expression.GetNextOccurrence(fromUtc, TimeZoneInfo.FindSystemTimeZoneById(DefaultTimeZone));

        if (nextRunAt == null)
        {
            error = "The cron expression never runs.";
            return false;
        }

        return true;
    }
}
