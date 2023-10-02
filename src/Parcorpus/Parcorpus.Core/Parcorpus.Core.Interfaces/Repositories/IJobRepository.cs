using Parcorpus.Core.Models;
using Parcorpus.Core.Models.Enums;

namespace Parcorpus.Core.Interfaces;

public interface IJobRepository
{
    Task<ProgressJob> AddJob(ProgressJob newJob);

    Task<ProgressJob> GetJobById(Guid jobId);

    Task<ProgressJob> UpdateStatus(Guid jobId, JobStatus newStatus);

    Task<List<ProgressJob>> GetUserJobs(Guid userId);
}