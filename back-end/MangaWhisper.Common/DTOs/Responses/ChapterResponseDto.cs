namespace MangaWhisper.Common.DTOs.Responses;

public class ChapterResponseDto
{
    public int Number { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ExtractedAt { get; set; } = string.Empty;
}

public class ChaptersListResponseDto
{
    public List<ChapterResponseDto> Chapters { get; set; } = new();
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public int TotalChapters { get; set; }
}