using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Mappers;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Queries;

public class GetAdminCheckersQueryHandler : IRequestHandler<GetAdminCheckersQuery, AdminCheckerListResponseDto>
{
    private readonly IMangaCheckerRepository _mangaCheckerRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly ILogger<GetAdminCheckersQueryHandler> _logger;

    public GetAdminCheckersQueryHandler(
        IMangaCheckerRepository mangaCheckerRepository,
        IChapterRepository chapterRepository,
        ILogger<GetAdminCheckersQueryHandler> logger)
    {
        _mangaCheckerRepository = mangaCheckerRepository;
        _chapterRepository = chapterRepository;
        _logger = logger;
    }

    public async Task<AdminCheckerListResponseDto> Handle(GetAdminCheckersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var checkers = await _mangaCheckerRepository.GetAllCheckersAsync();
            var chapterCounts = await _chapterRepository.GetCountsByMangaAsync();

            var response = checkers
                .OrderBy(c => c.Manga.Title)
                .Select(c => AdminCheckerMapper.ToDto(c, chapterCounts.TryGetValue(c.MangaId, out var count) ? count : 0))
                .ToList();

            return new AdminCheckerListResponseDto
            {
                Checkers = response,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving manga checkers for the admin panel");
            return new AdminCheckerListResponseDto
            {
                Success = false,
                ErrorMessage = "Failed to retrieve manga checkers from database"
            };
        }
    }
}
