using MangaWhisper.Domain.Entities;

namespace MangaWhisper.Domain.Entities;

public class ChapterCheckResult
{
    public bool Success { get; set; }
    public Chapter? NewChapter { get; set; }
    public string? ErrorMessage { get; set; }
    public bool RequiresSelenium { get; set; }
    public bool IsAntiBotDetected { get; set; }
    public bool ChapterExists { get; set; }

    // For when HTTP check confirms chapter exists (but no chapter data extracted)
    public static ChapterCheckResult SuccessHttpOnly(bool chapterExists)
    {
        return new ChapterCheckResult
        {
            Success = true,
            ChapterExists = chapterExists,
            NewChapter = null,
        };
    }

    // For when we have extracted the full chapter data (via Selenium)
    public static ChapterCheckResult SuccessWithChapter(Chapter? chapter)
    {
        return new ChapterCheckResult
        {
            Success = true,
            ChapterExists = chapter != null,
            NewChapter = chapter,
        };
    }

    // For when HTTP was successful, but needs Selenium to proceed
    public static ChapterCheckResult RequiresSeleniumCheck()
    {
        return new ChapterCheckResult
        {
            Success = true,
            RequiresSelenium = true,
            ChapterExists = false,
        };
    }

    // For failure scenarios
    public static ChapterCheckResult Failure(string errorMessage, bool requiresSelenium = false, bool isAntiBotDetected = false)
    {
        return new ChapterCheckResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            RequiresSelenium = requiresSelenium,
            IsAntiBotDetected = isAntiBotDetected,
            ChapterExists = false
        };
    }
}
