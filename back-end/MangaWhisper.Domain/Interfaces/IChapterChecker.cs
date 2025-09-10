using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Interfaces;

public interface IChapterChecker
{
    Task<Chapter?> GetNewChapter(MangaChecker subscription);
    void Dispose();
}
