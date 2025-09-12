using Microsoft.Extensions.Logging;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Factories;
using MangaWhisper.Domain.Repositories;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Application.Services;

public class ChapterCheckingService : IChapterCheckingService
{
    private readonly IChapterCheckerFactory _checkerFactory;
    private readonly ILogger<ChapterCheckingService> _logger;
    private readonly IMangaCheckerRepository _mangaCheckerRepository;
    private readonly IChapterRepository _chapterRepository;
    private readonly IMangaRepository _mangaRepository;

    public ChapterCheckingService(
        IChapterCheckerFactory checkerFactory,
        ILogger<ChapterCheckingService> logger,
        IMangaCheckerRepository mangaCheckerRepository,
        IChapterRepository chapterRepository,
        IMangaRepository mangaRepository)
    {
        _checkerFactory = checkerFactory;
        _logger = logger;
        _mangaCheckerRepository = mangaCheckerRepository;
        _chapterRepository = chapterRepository;
        _mangaRepository = mangaRepository;
    }

    public async Task<IEnumerable<MangaChecker>> GetActiveCheckersAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving active manga checkers");
            return await _mangaCheckerRepository.GetActiveCheckersAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving active manga checkers");
            throw;
        }
    }

    public async Task<IEnumerable<MangaChecker>> GetAllCheckersAsync()
    {
        try
        {
            _logger.LogInformation("Retrieving all manga checkers");
            return await _mangaCheckerRepository.GetAllCheckersAsync();
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

            // Check if manga already exists
            var existingManga = await _mangaRepository.GetByTitleAsync(checker.Manga.Title);

            if (existingManga != null)
            {
                checker.MangaId = existingManga.Id;
                checker.Manga = existingManga;
            }
            else
            {
                // Add new manga
                await _mangaRepository.AddAsync(checker.Manga);
                await _mangaRepository.SaveChangesAsync();
            }

            // Add the checker
            await _mangaCheckerRepository.AddAsync(checker);
            await _mangaCheckerRepository.SaveChangesAsync();

            _logger.LogInformation("Successfully added manga checker with ID: {CheckerId}", checker.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding manga checker for manga: {MangaTitle}", checker?.Manga?.Title);
            throw;
        }
    }

    public async Task<bool> HasNewChapterAsync(MangaChecker checker)
    {
        try
        {
            var chapterChecker = _checkerFactory.CreateChecker(checker.SiteIdentifier);
            return await chapterChecker.HasNewChapterAsync(checker);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if new chapter exists for checker {CheckerId}", checker.Id);
            return false;
        }
    }

    public async Task<Chapter?> ExtractNewChapterInfoAsync(MangaChecker checker)
    {
        try
        {
            var chapterChecker = _checkerFactory.CreateChecker(checker.SiteIdentifier);
            return await chapterChecker.ExtractNewChapterInfoAsync(checker);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting new chapter info for checker {CheckerId}", checker.Id);
            return null;
        }
    }

    public async Task UpdateCheckerStatusAsync(int checkerId, MangaCheckerStatus status)
    {
        try
        {
            await _mangaCheckerRepository.UpdateStatusAsync(checkerId, status);
            await _mangaCheckerRepository.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating checker status for checker {CheckerId}", checkerId);
            throw;
        }
    }

    private async Task SaveNewChapterAsync(Chapter newChapter, MangaChecker checker)
    {
        try
        {
            // Check if chapter already exists
            var existingChapter = await _chapterRepository.GetByMangaAndNumberAsync(newChapter.MangaId, newChapter.Number);

            if (existingChapter == null)
            {
                await _chapterRepository.AddAsync(newChapter);
                await _chapterRepository.SaveChangesAsync();

                // Update the checker's last known chapter
                await _mangaCheckerRepository.UpdateStatusAsync(checker.Id, checker.CheckerStatus);
                checker.LastKnownChapter = newChapter.Number;
                await _mangaCheckerRepository.SaveChangesAsync();

                _logger.LogInformation("Saved new chapter {ChapterNumber} for manga {MangaId}",
                    newChapter.Number, newChapter.MangaId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving new chapter {ChapterNumber} for manga {MangaId}",
                newChapter.Number, newChapter.MangaId);
            throw;
        }
    }
}