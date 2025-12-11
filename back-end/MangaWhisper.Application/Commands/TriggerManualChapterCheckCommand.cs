using MediatR;
using MangaWhisper.Common.DTOs.Responses;

namespace MangaWhisper.Application.Commands;

public record TriggerManualChapterCheckCommand() : IRequest<ManualCheckResponseDto>;
