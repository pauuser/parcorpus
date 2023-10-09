using Parcorpus.Core.Models;
using Parcorpus.Core.Models.Enums;
using Parcorpus.DataAccess.Models;
using Parcorpus.DataAccess.Models.Enums;

namespace Parcorpus.UnitTests.Common.Factories.CoreModels;

public static class JobFactory
{
    public static ProgressJob Create(Guid? jobId = null, 
        Guid? userId = null, 
        JobStatus status = JobStatus.Uploaded, 
        DateTime? startedTimeUtc = null)
    {
        return new ProgressJob(jobId: jobId ?? Guid.Empty,
            userId: userId ?? Guid.Empty,
            status,
            startedTimeUtc ?? new DateTime(2023, 9, 1));
    }

    public static ProgressJob Create(JobDbModel job)
    {
        return new ProgressJob(job.JobId, 
            job.UserId, 
            ConvertStatus(job.JobStatus),
            job.StartedTimeUtc);
    }
    
    private static JobStatus ConvertStatus(JobStatusDbModel status)
    {
        return status switch
        {
            JobStatusDbModel.Uploaded => JobStatus.Uploaded,
            JobStatusDbModel.Aligning => JobStatus.Aligning,
            JobStatusDbModel.Saving => JobStatus.Saving,
            JobStatusDbModel.Finished => JobStatus.Finished,
            JobStatusDbModel.Failed => JobStatus.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}