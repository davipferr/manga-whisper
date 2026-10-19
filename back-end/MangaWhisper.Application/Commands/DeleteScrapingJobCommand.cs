using MediatR;
using MangaWhisper.Common.DTOs.Responses.Admin;

namespace MangaWhisper.Application.Commands;

public record DeleteScrapingJobCommand(int JobId) : IRequest<ScrapingJobResultDto>;
