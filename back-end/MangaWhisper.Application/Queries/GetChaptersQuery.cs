using MediatR;
using MangaWhisper.Common.DTOs.Responses;

namespace MangaWhisper.Application.Queries;

public record GetChaptersQuery(int Page = 1, int PageSize = 5) : IRequest<ChaptersListResponseDto>;