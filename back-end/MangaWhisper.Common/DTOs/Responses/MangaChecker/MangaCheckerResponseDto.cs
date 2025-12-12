
namespace MangaWhisper.Common.DTOs.Responses.MangaChecker;

public class MangaCheckerResponseDto
{
    public int Id { get; set; }
    public int MangaId { get; set; }
    public string CheckerUrl { get; set; } = string.Empty;
    public string ChapterSelector { get; set; } = string.Empty;
    public string TitleSelector { get; set; } = string.Empty;
    public string UrlSelector { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class MangaCheckerListResponseDto
{
    public List<MangaCheckerResponseDto> MangaCheckers { get; set; } = new List<MangaCheckerResponseDto>();
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
