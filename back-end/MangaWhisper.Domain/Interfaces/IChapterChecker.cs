using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Interfaces;

public interface IChapterChecker
{
    Task<Chapter?> GetNewChapter(MangaSubscription subscription);
    void Dispose();
}
