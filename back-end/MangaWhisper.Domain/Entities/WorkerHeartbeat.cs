namespace MangaWhisper.Domain.Entities;

/// <summary>
/// Single row the worker updates periodically, so the admin panel can tell if it is running
/// </summary>
public class WorkerHeartbeat
{
    public const int SingletonId = 1;

    public int Id { get; set; }
    public DateTime LastSeenAt { get; set; }
    public DateTime StartedAt { get; set; }
    // Set when the worker shuts down cleanly, cleared by its next beat
    public DateTime? StoppedAt { get; set; }
}
