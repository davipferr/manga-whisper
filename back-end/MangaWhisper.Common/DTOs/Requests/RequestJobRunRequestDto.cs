using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Common.DTOs.Requests;

public class RequestJobRunRequestDto
{
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter<ScrapingJobType>))]
    public ScrapingJobType? Type { get; set; }
}
