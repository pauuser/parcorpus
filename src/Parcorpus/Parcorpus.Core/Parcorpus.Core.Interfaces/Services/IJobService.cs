using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface IJobService
{
    Task<ProgressJob> UploadText(Guid userId, BiText text);

    Task<List<ProgressJob>> GetUserJobs(Guid userId);

    Task<ProgressJob> GetJobById(Guid jobId);
}