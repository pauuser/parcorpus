using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Parcorpus.Core.Exceptions;
using Parcorpus.Core.Interfaces;
using Parcorpus.Core.Models;
using Parcorpus.Core.Models.Enums;
using Parcorpus.DataAccess.Context;
using Parcorpus.DataAccess.Converters;

namespace Parcorpus.DataAccess.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ILogger<JobRepository> _logger;
    private readonly ParcorpusDbContext _context;

    public JobRepository(ILogger<JobRepository> logger, ParcorpusDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ProgressJob> AddJob(ProgressJob newJob)
    {
        try
        {
            var jobDbModel = JobConverter.ConvertAppModelToDbModel(newJob);
            var entry = await _context.Jobs.AddAsync(jobDbModel);
            var entity = entry.Entity;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Job with Id {id} successfully added", entity.JobId);

            return JobConverter.ConvertDbModelToAppModel(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while adding job: {message}", ex.Message);
            throw new JobRepositoryException($"Error while adding job: {ex.Message}", ex);
        }
    }

    public async Task<ProgressJob> GetJobById(Guid jobId)
    {
        try
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == jobId);
            if (job is null)
            {
                _logger.LogError("Job with id {jobId} does not exist", jobId);
                throw new JobNotFoundException($"Job with id {jobId} does not exist");
            }
            _logger.LogInformation("Successfully retrieved job with id {jobId} in status {status}", jobId, job.JobStatus);

            return JobConverter.ConvertDbModelToAppModel(job);
        }
        catch (JobNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting job by id: {message}", ex.Message);
            throw new JobRepositoryException($"Error while getting job by id: {ex.Message}", ex);
        }
    }

    public async Task<ProgressJob> UpdateStatus(Guid jobId, JobStatus newStatus)
    {
        try
        {
            var job = await _context.Jobs.FirstOrDefaultAsync(j => j.JobId == jobId);
            if (job is null)
            {
                _logger.LogError("Job with id {jobId} does not exist", jobId);
                throw new JobNotFoundException($"Job with id {jobId} does not exist");
            }
            _logger.LogInformation("Successfully retrieved job with id {jobId} in status {status}", jobId, job.JobStatus);

            job.JobStatus = JobStatusConverter.ConvertAppModelToDbModel(newStatus);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Changes job {id} status to {status}", jobId, job.JobStatus);

            return JobConverter.ConvertDbModelToAppModel(job);
        }
        catch (JobNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while updating job status for job {id}: {message}", jobId, ex.Message);
            throw new JobRepositoryException($"Error while updating job status for job {jobId}: {ex.Message}", ex);
        }
    }

    public async Task<List<ProgressJob>> GetUserJobs(Guid userId)
    {
        try
        {
            var jobs = await _context.Jobs.Where(j => j.UserId == userId).ToListAsync();
            if (jobs is null)
            {
                _logger.LogError("Jobs of user id {userId} do not exist", userId);
                throw new JobNotFoundException($"Jobs with user id {userId} do not exist");
            }
            _logger.LogInformation("Successfully retrieved jobs of userId {userId}, count: {count}", userId, jobs.Count);

            return jobs.Select(JobConverter.ConvertDbModelToAppModel).ToList();
        }
        catch (JobNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while getting user jobs for user {userId}: {message}", userId, ex.Message);
            throw new JobRepositoryException($"Error while getting user jobs for user {userId}: {ex.Message}", ex);
        }
    }
}