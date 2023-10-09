using Parcorpus.Core.Models.Enums;

namespace Parcorpus.Core.Models;

public sealed class ProgressJob : IEquatable<ProgressJob>
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

    public override bool Equals(object? obj)
    {
        if (obj is ProgressJob job)
            return Equals(job);
        
        return base.Equals(obj);
    }

    public bool Equals(ProgressJob? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return JobId.Equals(other.JobId) && 
               UserId.Equals(other.UserId) && 
               Status == other.Status && 
               StartedTimeUtc.Equals(other.StartedTimeUtc);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(JobId, UserId, (int)Status, StartedTimeUtc);
    }
}