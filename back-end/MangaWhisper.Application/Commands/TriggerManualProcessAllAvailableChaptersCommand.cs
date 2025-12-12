using MediatR;
using MangaWhisper.Common.DTOs.Responses;

namespace MangaWhisper.Application.Commands;

public record TriggerManualProcessAllAvailableChaptersCommand(int CheckerId) : IRequest<ManualCheckResponseDto>;
