using Parcorpus.DataAccess.Models.Enums;

namespace Parcorpus.DataAccess.Models;

public class JobDbModel
{
    public Guid JobId { get; set; }

    public Guid UserId { get; set; }

    public virtual UserDbModel UserNavigation { get; set; }

    public JobStatusDbModel JobStatus { get; set; }

    public DateTime StartedTimeUtc { get; set; }

    public JobDbModel(Guid jobId, Guid userId, JobStatusDbModel jobStatus, DateTime startedTimeUtc)
    {
        JobId = jobId;
        UserId = userId;
        JobStatus = jobStatus;
        StartedTimeUtc = startedTimeUtc;
    }

    public JobDbModel()
    {
    }
}