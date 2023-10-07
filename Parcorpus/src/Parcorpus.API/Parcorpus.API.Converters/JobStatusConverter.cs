using Parcorpus.API.Dto.Enums;
using Parcorpus.Core.Models.Enums;

namespace Parcorpus.API.Converters;

public static class JobStatusConverter
{
    public static JobStatusDto ConvertAppModelToDto(JobStatus status)
    {
        return status switch
        {
            JobStatus.Uploaded => JobStatusDto.Uploaded,
            JobStatus.Aligning => JobStatusDto.Aligning,
            JobStatus.Saving => JobStatusDto.Saving,
            JobStatus.Finished => JobStatusDto.Finished,
            JobStatus.Failed => JobStatusDto.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}