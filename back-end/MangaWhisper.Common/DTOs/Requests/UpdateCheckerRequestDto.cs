using System.ComponentModel.DataAnnotations;

namespace MangaWhisper.Common.DTOs.Requests;

public class UpdateCheckerRequestDto
{
    public bool? IsActive { get; set; }

    [Range(0, 100000)]
    public int? LastKnownChapter { get; set; }
}
