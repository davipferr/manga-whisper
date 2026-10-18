using MediatR;
using MangaWhisper.Common.DTOs.Responses.Admin;

namespace MangaWhisper.Application.Commands;

public record UpdateCheckerCommand(int CheckerId, bool? IsActive, int? LastKnownChapter) : IRequest<AdminCheckerResultDto>;
