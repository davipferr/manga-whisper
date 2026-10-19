using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Common.DTOs.Requests;

public class SaveScrapingJobRequestDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int MangaCheckerId { get; set; }

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter<ScrapingJobType>))]
    public ScrapingJobType? Type { get; set; }

    /// <summary>
    /// Standard 5-field cron expression (minute hour day-of-month month day-of-week), evaluated in BRT
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string CronExpression { get; set; } = string.Empty;

    public bool IsEnabled { get; set; } = true;
}
