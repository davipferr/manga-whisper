using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Entities;

public class MangaInfo
{
    public string Title { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public MangaStatus Status { get; set; }
    public float? LatestChapterNumber { get; set; }
    public string BaseUrl { get; set; } = string.Empty;
}
