using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Commands;
using MangaWhisper.Application.Services;
using MangaWhisper.Common.DTOs.Responses;

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

            await _chapterCheckingService.CheckAllActiveCheckersManuallyAsync(cancellationToken);

            return new ManualCheckResponseDto
            {
                Success = true,
                Message = "Manual chapter check completed successfully"
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
}
