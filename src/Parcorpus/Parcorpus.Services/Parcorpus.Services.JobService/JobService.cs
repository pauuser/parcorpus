using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Parcorpus.Core.Configuration;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Core.Models.Enums;

namespace Parcorpus.Services.JobService;

public class JobService : IJobService
{
    private readonly IQueueProducerService _producerService;
    private readonly IJobRepository _jobRepository;
    private readonly PagingConfiguration _pagingConfiguration;

    private readonly ILogger<JobService> _logger;

    public JobService(IQueueProducerService producerService, 
        IJobRepository jobRepository, 
        IOptions<PagingConfiguration> pagingConfiguration,
        ILogger<JobService> logger)
    {
        _producerService = producerService ?? throw new ArgumentNullException(nameof(producerService));
        _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
        _pagingConfiguration = pagingConfiguration.Value ?? throw new ArgumentNullException(nameof(pagingConfiguration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<ProgressJob> UploadText(Guid userId, BiText text)
    {
        var job = await _jobRepository.AddJob(new ProgressJob(jobId: Guid.NewGuid(),
            userId: userId,
            status: JobStatus.Uploaded,
            startedTimeUtc: DateTime.UtcNow));

        await _producerService.SendMessage(new UploadJob(userId: userId,
            biText: text,
            jobId: job.JobId));
        
        return job;
    }

    public async Task<Paged<ProgressJob>> GetUserJobs(Guid userId, PaginationParameters paging)
    {
        if (paging.Specified && (paging.PageSize < _pagingConfiguration.MinPageSize || paging.PageSize > _pagingConfiguration.MaxPageSize))
        {
            _logger.LogError("Invalid paging to get user {userId} jobs: {paging}", userId, paging);
            throw new InvalidPagingException($"Invalid paging to get user {userId} jobs: {paging}");
        }
        
        var jobs = await _jobRepository.GetUserJobs(userId, paging);

        return jobs;
    }

    public Task<ProgressJob> GetJobById(Guid jobId)
    {
        return _jobRepository.GetJobById(jobId);
    }
}