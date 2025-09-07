using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Interfaces;

public interface IChapterChecker
{
    Task<ChapterCheckResult> CheckForNewChapter(MangaSubscription subscription);
    bool ValidateMangaUrl(string url);
    Task<MangaInfo?> ExtractMangaInfo(string url);
    string GetSiteName();
}
