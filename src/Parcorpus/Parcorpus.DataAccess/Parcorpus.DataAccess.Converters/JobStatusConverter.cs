using Parcorpus.Core.Models.Enums;
using Parcorpus.DataAccess.Models.Enums;

namespace Parcorpus.DataAccess.Converters;

public static class JobStatusConverter
{
    public static JobStatus ConvertDbModelToAppModel(JobStatusDbModel status)
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
    
    public static JobStatusDbModel ConvertAppModelToDbModel(JobStatus status)
    {
        return status switch
        {
            JobStatus.Uploaded => JobStatusDbModel.Uploaded,
            JobStatus.Aligning => JobStatusDbModel.Aligning,
            JobStatus.Saving => JobStatusDbModel.Saving,
            JobStatus.Finished => JobStatusDbModel.Finished,
            JobStatus.Failed => JobStatusDbModel.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    }
}