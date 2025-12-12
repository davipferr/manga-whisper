using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Queries;
using MangaWhisper.Common.DTOs.Responses.MangaChecker;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Queries;

public class GetMangaCheckerByMangaTitleQueryHandler : IRequestHandler<GetMangaCheckerByMangaTitleQuery, MangaCheckerListResponseDto>
{
    private readonly IMangaCheckerRepository _mangaCheckerRepository;
    private readonly ILogger<GetMangaCheckerByMangaTitleQueryHandler> _logger;

    public GetMangaCheckerByMangaTitleQueryHandler(
        IMangaCheckerRepository mangaCheckerRepository,
        ILogger<GetMangaCheckerByMangaTitleQueryHandler> logger)
    {
        _mangaCheckerRepository = mangaCheckerRepository;
        _logger = logger;
    }

    public async Task<MangaCheckerListResponseDto> Handle(GetMangaCheckerByMangaTitleQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var mangaCheckers = await _mangaCheckerRepository.GetByMangaTitleAsync(request.MangaTitle);

            var response = mangaCheckers.Select(mc => new MangaCheckerResponseDto
            {
                Id = mc.Id,
                MangaId = mc.Manga.Id
            }).ToList();

            return new MangaCheckerListResponseDto
            {
                MangaCheckers = response,
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chapters from database");
            return new MangaCheckerListResponseDto
            {
                Success = false,
                ErrorMessage = "Failed to retrieve chapters from database"
            };
        }
    }
}