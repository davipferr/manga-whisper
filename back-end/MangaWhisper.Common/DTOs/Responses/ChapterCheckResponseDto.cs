namespace MangaWhisper.Common.DTOs.Responses;

public class ChapterCheckResponseDto
{
    public bool Success { get; set; }
    public bool ChapterExists { get; set; }
    public ChapterDto? NewChapter { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RequiresSelenium { get; set; }
    public bool IsAntiBotDetected { get; set; }
}

public class ChapterDto
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
