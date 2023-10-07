using Parcorpus.Core.Models.Enums;

namespace Parcorpus.Core.Models;

public sealed class ProgressJob
{
    public Guid JobId { get; set; }

    public Guid UserId { get; set; }

    public JobStatus Status { get; set; }

    public DateTime StartedTimeUtc { get; set; }

    public ProgressJob(Guid jobId, Guid userId, JobStatus status, DateTime startedTimeUtc)
    {
        JobId = jobId;
        UserId = userId;
        Status = status;
        StartedTimeUtc = startedTimeUtc;
    }
}