namespace MangaWhisper.Common.DTOs.Responses;

public class TestCheckerResponseDto
{
    public string SiteName { get; set; } = string.Empty;
    public bool IsUrlValid { get; set; }
    public MangaInfoResponseDto? MangaInfo { get; set; }
    public ChapterCheckResponseDto? ChapterCheck { get; set; }
    public string? ErrorMessage { get; set; }
    public bool Success { get; set; }
}
