using MangaWhisper.Common.Enums;

namespace MangaWhisper.Common.DTOs.Responses;

public class MangaInfoResponseDto
{
    public string Title { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public MangaStatus Status { get; set; }
    public int? LatestChapterNumber { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
}
