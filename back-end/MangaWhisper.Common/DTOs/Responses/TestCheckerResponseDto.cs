namespace MangaWhisper.Common.DTOs.Responses;

public class TestCheckerResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<CheckerInfoDto> Checkers { get; set; } = new();
}

public class CheckerInfoDto
{
    public int Id { get; set; }
    public string SiteIdentifier { get; set; } = string.Empty;
    public string MangaTitle { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int CheckIntervalMinutes { get; set; }
}