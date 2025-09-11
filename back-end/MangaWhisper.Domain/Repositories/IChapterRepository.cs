using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Repositories;

public interface IChapterRepository
{
    Task<Chapter?> GetByMangaAndNumberAsync(int mangaId, decimal number);
    Task AddAsync(Chapter chapter);
    Task SaveChangesAsync();
}
