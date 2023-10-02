using Microsoft.Extensions.Logging;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Core.Models.Enums;

namespace Parcorpus.Services.JobService;

public class JobService : IJobService
{
    private readonly IQueueProducerService _producerService;
    private readonly IJobRepository _jobRepository;

    private readonly ILogger<JobService> _logger;

    public JobService(IQueueProducerService producerService, 
        IJobRepository jobRepository, 
        ILogger<JobService> logger)
    {
        _producerService = producerService ?? throw new ArgumentNullException(nameof(producerService));
        _jobRepository = jobRepository ?? throw new ArgumentNullException(nameof(jobRepository));
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

    public Task<List<ProgressJob>> GetUserJobs(Guid userId)
    {
        return _jobRepository.GetUserJobs(userId);
    }

    public Task<ProgressJob> GetJobById(Guid jobId)
    {
        return _jobRepository.GetJobById(jobId);
    }
}