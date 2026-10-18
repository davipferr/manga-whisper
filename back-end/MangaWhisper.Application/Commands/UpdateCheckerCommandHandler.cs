using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Mappers;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Commands;

public class UpdateCheckerCommandHandler : IRequestHandler<UpdateCheckerCommand, AdminCheckerResultDto>
{
    private readonly IMangaCheckerRepository _mangaCheckerRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly ILogger<UpdateCheckerCommandHandler> _logger;

    public UpdateCheckerCommandHandler(
        IMangaCheckerRepository mangaCheckerRepository,
        IChapterRepository chapterRepository,
        ILogger<UpdateCheckerCommandHandler> logger)
    {
        _mangaCheckerRepository = mangaCheckerRepository;
        _chapterRepository = chapterRepository;
        _logger = logger;
    }

    public async Task<AdminCheckerResultDto> Handle(UpdateCheckerCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var checker = await _mangaCheckerRepository.GetByIdAsync(request.CheckerId);
            if (checker == null)
            {
                return new AdminCheckerResultDto
                {
                    Success = false,
                    NotFound = true,
                    ErrorMessage = $"Checker {request.CheckerId} does not exist"
                };
            }

            if (request.IsActive == true)
            {
                checker.Resume();
            }
            else if (request.IsActive == false)
            {
                checker.Pause();
            }

            if (request.LastKnownChapter.HasValue)
            {
                checker.SetLastKnownChapter(request.LastKnownChapter.Value);
            }

            await _mangaCheckerRepository.SaveChangesAsync();

            var chapterCounts = await _chapterRepository.GetCountsByMangaAsync();
            var storedChapters = chapterCounts.TryGetValue(checker.MangaId, out var count) ? count : 0;

            _logger.LogInformation("Checker {CheckerId} updated: IsActive={IsActive}, LastKnownChapter={LastKnownChapter}",
                checker.Id, checker.IsActive, checker.LastKnownChapter);

            return new AdminCheckerResultDto
            {
                Checker = AdminCheckerMapper.ToDto(checker, storedChapters),
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating checker {CheckerId}", request.CheckerId);
            return new AdminCheckerResultDto
            {
                Success = false,
                ErrorMessage = "Failed to update the checker"
            };
        }
    }
}
