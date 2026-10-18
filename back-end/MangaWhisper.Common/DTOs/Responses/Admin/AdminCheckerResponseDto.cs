namespace MangaWhisper.Common.DTOs.Responses.Admin;

public class AdminCheckerResponseDto
{
    public int Id { get; set; }
    public int MangaId { get; set; }
    public string MangaTitle { get; set; } = string.Empty;
    public string SiteIdentifier { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public int LastKnownChapter { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public int StoredChapters { get; set; }
}

public class AdminCheckerListResponseDto
{
    public List<AdminCheckerResponseDto> Checkers { get; set; } = new();
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AdminCheckerResultDto
{
    public AdminCheckerResponseDto? Checker { get; set; }
    public bool Success { get; set; }
    public bool NotFound { get; set; }
    public string? ErrorMessage { get; set; }
}
