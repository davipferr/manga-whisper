using Microsoft.Extensions.Logging;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Factories;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Application.Services;

public class ChapterCheckingService : IChapterCheckingService
{
    private readonly IChapterCheckerFactory _checkerFactory;
    private readonly ILogger<ChapterCheckingService> _logger;
    // Add your repository/DbContext here for data access

    public ChapterCheckingService(
        IChapterCheckerFactory checkerFactory,
        ILogger<ChapterCheckingService> logger)
    {
        _checkerFactory = checkerFactory;
        _logger = logger;
    }

    public async Task<IEnumerable<MangaChecker>> GetActiveCheckersAsync()
    {
        // Implement using your data access layer
        // Return all active MangaChecker entities that should be checked
        throw new NotImplementedException("Implement with your data access layer");
    }

    public async Task<IEnumerable<MangaChecker>> GetAllCheckersAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all manga checkers");
            
            // Implement using your data access layer
            // Return all MangaChecker entities
            throw new NotImplementedException("Implement with your data access layer");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all manga checkers");
            throw;
        }
    }

    public async Task AddCheckerAsync(MangaChecker checker)
    {
        try
        {
            _logger.LogInformation("Adding new manga checker for manga: {MangaTitle}", checker.Manga.Title);
            
            // Validate the checker
            if (checker == null)
                throw new ArgumentNullException(nameof(checker));
            
            if (string.IsNullOrWhiteSpace(checker.Manga.Title))
                throw new ArgumentException("Manga title cannot be empty", nameof(checker));

            // Implement using your data access layer
            // Add the checker to the database
            throw new NotImplementedException("Implement with your data access layer");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding manga checker for manga: {MangaTitle}", checker?.Manga.Title);
            throw;
        }
    }

    public async Task<Chapter?> CheckForNewChapterAsync(MangaChecker checker)
    {
        try
        {
            using var chapterChecker = _checkerFactory.CreateChecker(checker.SiteIdentifier);
            return await chapterChecker.GetNewChapter(checker);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking for new chapter for checker {CheckerId}", checker.Id);
            return null;
        }
    }

    public async Task UpdateCheckerStatusAsync(int checkerId, MangaCheckerStatus status)
    {
        // Implement using your data access layer
        throw new NotImplementedException("Implement with your data access layer");
    }
}