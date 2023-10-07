using Parcorpus.API.Dto;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Converters;

public static class JobConverter
{
    public static JobDto ConvertAppModelToDto(ProgressJob job)
    {
        return new JobDto(jobId: job.JobId,
            jobStatus: JobStatusConverter.ConvertAppModelToDto(job.Status));
    }
}