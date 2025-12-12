using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Commands;
using MangaWhisper.Application.Services;
using MangaWhisper.Common.DTOs.Responses;

namespace MangaWhisper.Application.Queries;

public class TriggerManualProcessAllAvailableChaptersCommandHandler : IRequestHandler<TriggerManualProcessAllAvailableChaptersCommand, ManualCheckResponseDto>
{
    private readonly IChapterCheckingService _chapterCheckingService;
    private readonly ILogger<TriggerManualProcessAllAvailableChaptersCommandHandler> _logger;

    public TriggerManualProcessAllAvailableChaptersCommandHandler(
        IChapterCheckingService chapterCheckingService,
        ILogger<TriggerManualProcessAllAvailableChaptersCommandHandler> logger)
    {
        _chapterCheckingService = chapterCheckingService;
        _logger = logger;
    }

    public async Task<ManualCheckResponseDto> Handle(TriggerManualProcessAllAvailableChaptersCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Manual chapter check triggered via API for checker ID: {CheckerId}", request.CheckerId);

            var newChapters = await _chapterCheckingService.ProcessAllAvailableChaptersForCheckerAsync(request.CheckerId, cancellationToken);

            return new ManualCheckResponseDto
            {
                Success = true,
                Message = "Process all chapters available completed successfully",
                NewChapters = newChapters.Select(c =>
                new ChapterResponseDto {
                    MangaId = c.MangaId,
                    Title = c.Title,
                    Number = c.Number
                }).ToList()
            };
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Process all chapters check was cancelled");
            return new ManualCheckResponseDto
            {
                Success = false,
                ErrorMessage = "Process all chapters check was cancelled"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during process all chapters");
            return new ManualCheckResponseDto
            {
                Success = false,
                ErrorMessage = "An error occurred during process all chapters"
            };
        }
    }
}
