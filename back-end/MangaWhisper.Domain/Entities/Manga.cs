using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Entities;

public class Manga
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public MangaStatus Status { get; set; }
    public string CoverImageUrl { get; set; } = string.Empty;

    public virtual ICollection<Chapter> Chapters { get; set; } = new List<Chapter>();
    public virtual ICollection<MangaSubscription> Subscriptions { get; set; } = new List<MangaSubscription>();

    public Chapter? GetLatestChapter()
    {
        return Chapters
            .OrderByDescending(c => c.Number)
            .FirstOrDefault();
    }
}
