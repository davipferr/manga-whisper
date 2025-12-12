namespace MangaWhisper.Common.DTOs.Responses;

public class ManualCheckResponseDto
{
    public List<ChapterResponseDto> NewChapters { get; set; } = new List<ChapterResponseDto>();
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
}
