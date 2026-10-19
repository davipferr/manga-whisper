namespace MangaWhisper.Common.DTOs.Responses.Admin;

public class ScrapingJobResponseDto
{
    public int Id { get; set; }
    public int MangaCheckerId { get; set; }
    public string MangaTitle { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string CronExpression { get; set; } = string.Empty;
    public string TimeZone { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public DateTime? NextRunAt { get; set; }
    public DateTime? LastRunAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ScrapingJobListResponseDto
{
    public List<ScrapingJobResponseDto> Jobs { get; set; } = new();
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ScrapingJobResultDto
{
    public ScrapingJobResponseDto? Job { get; set; }
    public bool Success { get; set; }
    public bool NotFound { get; set; }
    public bool Invalid { get; set; }
    public string? ErrorMessage { get; set; }
}
