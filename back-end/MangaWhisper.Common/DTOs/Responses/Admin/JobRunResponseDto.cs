namespace MangaWhisper.Common.DTOs.Responses.Admin;

public class JobRunResponseDto
{
    public int Id { get; set; }
    public int MangaCheckerId { get; set; }
    public string MangaTitle { get; set; } = string.Empty;
    // Null when the run was requested with "Run now" (or its job was deleted)
    public int? ScrapingJobId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime RequestedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public int ChaptersFound { get; set; }
    // What a successful run found; failed and cancelled runs explain themselves in ErrorMessage
    public string? ResultMessage { get; set; }
    public string? ErrorMessage { get; set; }
}

public class JobRunListResponseDto
{
    public List<JobRunResponseDto> Runs { get; set; } = new();
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class JobRunResultDto
{
    public JobRunResponseDto? Run { get; set; }
    public bool Success { get; set; }
    public bool NotFound { get; set; }
    public bool Conflict { get; set; }
    public string? ErrorMessage { get; set; }
}

public class WorkerStatusResponseDto
{
    public bool IsOnline { get; set; }
    public DateTime? LastSeenAt { get; set; }
    public DateTime? StartedAt { get; set; }
    // The run the worker is executing; null while it is idle or not running
    public JobRunResponseDto? CurrentRun { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
