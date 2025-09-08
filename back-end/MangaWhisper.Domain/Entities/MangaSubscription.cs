namespace MangaWhisper.Domain.Entities;

public class MangaSubscription
{
    public int Id { get; set; }
    public int MangaId { get; set; }
    public int SourceId { get; set; }
    public int UserId { get; set; }
    public string MangaBaseUrl { get; set; } = string.Empty;
    public int LastKnownChapter { get; set; }
    public int CheckIntervalMinutes { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public virtual Manga Manga { get; set; } = null!;
    public virtual MangaSource Source { get; set; } = null!;
    public virtual User User { get; set; } = null!;

    public int GetExpectedNextChapter()
    {
        return LastKnownChapter + 1;
    }

    public void UpdateLastKnownChapter(int chapter)
    {
        LastKnownChapter = chapter;
        LastCheckedAt = DateTime.UtcNow;
    }

    public bool ShouldCheck()
    {
        if (!IsActive) return false;
        
        if (!LastCheckedAt.HasValue) return true;
        
        var nextCheckTime = LastCheckedAt.Value.AddMinutes(CheckIntervalMinutes);
        return DateTime.UtcNow >= nextCheckTime;
    }
}
