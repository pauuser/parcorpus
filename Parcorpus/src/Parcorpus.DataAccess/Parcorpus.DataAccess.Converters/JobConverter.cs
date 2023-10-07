using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class JobConverter
{
    public static JobDbModel ConvertAppModelToDbModel(ProgressJob job)
    {
        return new JobDbModel(job.JobId, 
            job.UserId, 
            JobStatusConverter.ConvertAppModelToDbModel(job.Status),
            job.StartedTimeUtc);
    }
    
    public static ProgressJob ConvertDbModelToAppModel(JobDbModel job)
    {
        return new ProgressJob(job.JobId, 
            job.UserId, 
            JobStatusConverter.ConvertDbModelToAppModel(job.JobStatus),
            job.StartedTimeUtc);
    }
}