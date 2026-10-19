using MediatR;
using Microsoft.Extensions.Logging;
using MangaWhisper.Application.Mappers;
using MangaWhisper.Common.DTOs.Responses.Admin;
using MangaWhisper.Domain.Entities;
using MangaWhisper.Domain.Repositories;

namespace MangaWhisper.Application.Commands;

public class RequestJobRunCommandHandler : IRequestHandler<RequestJobRunCommand, JobRunResultDto>
{
    private readonly IJobRunRepository _jobRunRepository;
    private readonly IMangaCheckerRepository _mangaCheckerRepository;
    private readonly ILogger<RequestJobRunCommandHandler> _logger;

    public RequestJobRunCommandHandler(
        IJobRunRepository jobRunRepository,
        IMangaCheckerRepository mangaCheckerRepository,
        ILogger<RequestJobRunCommandHandler> logger)
    {
        _jobRunRepository = jobRunRepository;
        _mangaCheckerRepository = mangaCheckerRepository;
        _logger = logger;
    }

    public async Task<JobRunResultDto> Handle(RequestJobRunCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var checker = await _mangaCheckerRepository.GetByIdAsync(request.CheckerId);
            if (checker == null)
            {
                return new JobRunResultDto
                {
                    Success = false,
                    NotFound = true,
                    ErrorMessage = $"Checker {request.CheckerId} does not exist"
                };
            }

            if (await _jobRunRepository.HasOpenRunForCheckerAsync(checker.Id, cancellationToken))
            {
                return new JobRunResultDto
                {
                    Success = false,
                    Conflict = true,
                    ErrorMessage = "This checker already has a run waiting or in progress"
                };
            }

            var run = JobRun.RequestNow(checker.Id, request.Type);
            run.MangaChecker = checker;

            await _jobRunRepository.AddAsync(run, cancellationToken);
            await _jobRunRepository.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Run {RunId} ({Type}) requested for checker {CheckerId}", run.Id, run.Type, checker.Id);

            return new JobRunResultDto
            {
                Run = AdminJobMapper.ToDto(run),
                Success = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error requesting a run for checker {CheckerId}", request.CheckerId);
            return new JobRunResultDto
            {
                Success = false,
                ErrorMessage = "Failed to request the run"
            };
        }
    }
}
