using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Application.Mappers;

public static class AdminCheckerMapper
{
    public static AdminCheckerResponseDto ToDto(MangaChecker checker, int storedChapters)
    {
        return new AdminCheckerResponseDto
        {
            Id = checker.Id,
            MangaId = checker.MangaId,
            MangaTitle = checker.Manga?.Title ?? string.Empty,
            SiteIdentifier = checker.SiteIdentifier,
            IsActive = checker.IsActive,
            Status = checker.CheckerStatus.ToString(),
            LastKnownChapter = checker.LastKnownChapter,
            LastCheckedAt = checker.LastCheckedAt,
            StoredChapters = storedChapters
        };
    }
}
