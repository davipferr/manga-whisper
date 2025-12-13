using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Queries;
using MangaWhisper.Common.DTOs.Responses;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Queries;

public class GetChaptersQueryHandler : IRequestHandler<GetChaptersQuery, ChaptersListResponseDto>
{
    private readonly IChapterRepository _chapterRepository;
    private readonly ILogger<GetChaptersQueryHandler> _logger;

    public GetChaptersQueryHandler(
        IChapterRepository chapterRepository,
        ILogger<GetChaptersQueryHandler> logger)
    {
        _chapterRepository = chapterRepository;
        _logger = logger;
    }

    public async Task<ChaptersListResponseDto> Handle(GetChaptersQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var chapters = await _chapterRepository.GetPaginatedAsync(request.Page, request.PageSize);
            
            var totalChapters = await _chapterRepository.GetTotalCountAsync();

            var chapterDtos = chapters.Select(chapter => new ChapterResponseDto
            {
                Number = chapter.Number,
                Title = chapter.Title,
                Url = chapter.Url,
                ExtractedAt = chapter.ExtractedAt.ToString("dd/MM/yyyy")
            }).ToList();

            return new ChaptersListResponseDto
            {
                Chapters = chapterDtos,
                Success = true,
                TotalChapters = totalChapters
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving chapters from database");
            return new ChaptersListResponseDto
            {
                Success = false,
                ErrorMessage = "Failed to retrieve chapters from database"
            };
        }
    }
}