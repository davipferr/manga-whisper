using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Commands;
using MangaWhisper.Application.Services;
using MangaWhisper.Common.DTOs.Responses;
using System.Collections;

namespace MangaWhisper.Application.Queries;

public class TriggerManualChapterCheckCommandHandler : IRequestHandler<TriggerManualChapterCheckCommand, ManualCheckResponseDto>
{
    private readonly IChapterCheckingService _chapterCheckingService;
    private readonly ILogger<TriggerManualChapterCheckCommandHandler> _logger;

    public TriggerManualChapterCheckCommandHandler(
        IChapterCheckingService chapterCheckingService,
        ILogger<TriggerManualChapterCheckCommandHandler> logger)
    {
        _chapterCheckingService = chapterCheckingService;
        _logger = logger;
    }

    public async Task<ManualCheckResponseDto> Handle(TriggerManualChapterCheckCommand request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Manual chapter check triggered via API");

            var newChapters = await _chapterCheckingService.CheckAllActiveCheckersManuallyAsync(cancellationToken, true);

            return new ManualCheckResponseDto
            {
                Success = true,
                Message = FormatResponseMessage(newChapters),
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
            _logger.LogWarning("Manual chapter check was cancelled");
            return new ManualCheckResponseDto
            {
                Success = false,
                ErrorMessage = "Manual chapter check was cancelled"
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during manual chapter check");
            return new ManualCheckResponseDto
            {
                Success = false,
                ErrorMessage = "An error occurred during manual chapter check"
            };
        }
    }

    // TODO: Move to a shared utility class
    private string FormatResponseMessage<T>(IEnumerable<T> list)
    {
        int newChaptersCount = list.Count();

        string successfullMessage = "Manual chapter check completed successfully.";

        if (newChaptersCount == 0)
        {
            return $"{successfullMessage} No new chapters found during manual check.";
        }
        else if (newChaptersCount == 1)
        {
            return $"{successfullMessage} 1 new chapter found during manual check.";
        }
        else
        {
            return $"{successfullMessage} {newChaptersCount} new chapters found during manual check.";
        }
    }
}
