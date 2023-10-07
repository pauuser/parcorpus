using Parcorpus.Core.Models;

namespace Parcorpus.Core.Interfaces;

public interface IJobService
{
    Task<ProgressJob> UploadText(Guid userId, BiText text);

    Task<Paged<ProgressJob>> GetUserJobs(Guid userId, PaginationParameters paging);

    Task<ProgressJob> GetJobById(Guid jobId);
}