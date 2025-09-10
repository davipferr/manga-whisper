using MangaWhisper.Domain.Interfaces;

namespace MangaWhisper.Domain.Factories;

public interface IChapterCheckerFactory
{
    IChapterChecker CreateChecker(string siteIdentifier);
    IEnumerable<string> GetAvailableSites();
}