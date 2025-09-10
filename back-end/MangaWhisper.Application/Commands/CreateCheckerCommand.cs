using MediatR;
using MangaWhisper.Common.DTOs.Responses;
using MangaWhisper.Common.Enums;

namespace MangaWhisper.Application.Commands;

public record CreateCheckerCommand(
    string MangaTitle,
    string SiteIdentifier,
    int CheckIntervalMinutes,
    int LastKnownChapter
) : IRequest<CheckerInfoDto>;