using MangaWhisper.Domain.Entities;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Application.Services;

public interface IChapterCheckingService
{
    Task<IEnumerable<MangaChecker>> GetActiveCheckersAsync();
    Task<IEnumerable<MangaChecker>> GetAllCheckersAsync();
    Task<Chapter?> CheckForNewChapterAsync(MangaChecker checker);
    Task UpdateCheckerStatusAsync(int checkerId, MangaCheckerStatus status);
    Task AddCheckerAsync(MangaChecker checker);
}