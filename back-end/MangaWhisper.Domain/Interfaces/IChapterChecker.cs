using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Interfaces;

public interface IChapterChecker : IDisposable
{
    string SiteIdentifier { get; }
    Task<Chapter?> GetNewChapter(MangaChecker subscription);
}
