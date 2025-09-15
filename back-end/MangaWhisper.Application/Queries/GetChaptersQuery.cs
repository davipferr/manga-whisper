using MediatR;
using MangaWhisper.Common.DTOs.Responses;

namespace MangaWhisper.Application.Queries;

public record GetChaptersQuery() : IRequest<ChaptersListResponseDto>;