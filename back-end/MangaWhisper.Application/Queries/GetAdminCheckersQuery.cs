using MediatR;
using MangaWhisper.Common.DTOs.Responses.Admin;

namespace MangaWhisper.Application.Queries;

public record GetAdminCheckersQuery : IRequest<AdminCheckerListResponseDto>;
