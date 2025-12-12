using MediatR;
using MangaWhisper.Common.DTOs.Responses.MangaChecker;

namespace MangaWhisper.Application.Queries;

public record GetMangaCheckerByMangaTitleQuery(string MangaTitle) : IRequest<MangaCheckerListResponseDto>;