using MangaWhisper.Common.Enums;

namespace MangaWhisper.Domain.Entities;

public class MangaChecker
{
    public int Id { get; set; }
    public int MangaId { get; set; }
    public int LastKnownChapter { get; set; }
    public bool IsActive { get; set; }
    public MangaCheckerStatus CheckerStatus { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public string SiteIdentifier { get; set; } = string.Empty;

    public virtual Manga Manga { get; set; } = null!;

    public int GetExpectedNextChapter()
    {
        return LastKnownChapter + 1;
    }

    public void Pause()
    {
        IsActive = false;
        CheckerStatus = MangaCheckerStatus.Stopped;
    }

    public void Resume()
    {
        IsActive = true;
        CheckerStatus = MangaCheckerStatus.Idle;
    }

    public void SetLastKnownChapter(int chapter)
    {
        if (chapter < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(chapter), "Chapter number cannot be negative.");
        }

        LastKnownChapter = chapter;
    }
}
