namespace MangaWhisper.Domain.Entities;

public class HttpCheckResult
{
    public bool Success { get; set; }
    public bool ChapterExists { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }
    public bool RequiresSelenium { get; set; }

    public static HttpCheckResult SuccessResult(bool chapterExists)
    {
        return new HttpCheckResult
        {
            Success = true,
            ChapterExists = chapterExists,
        };
    }

    public static HttpCheckResult FailureResult(string errorMessage, int statusCode = 0)
    {
        return new HttpCheckResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            StatusCode = statusCode
        };
    }
}
