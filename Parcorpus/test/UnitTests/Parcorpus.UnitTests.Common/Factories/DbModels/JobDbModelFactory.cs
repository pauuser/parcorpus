using Parcorpus.Core.Models.Enums;
using Parcorpus.DataAccess.Models;
using Parcorpus.DataAccess.Models.Enums;

namespace Parcorpus.UnitTests.Common.Factories.DbModels;

public static class JobDbModelFactory
{
    public static JobDbModel Create(Guid? jobId = null,
        Guid? userId = null,
        JobStatusDbModel jobStatus = JobStatusDbModel.Uploaded,
        DateTime? startedTimeUtc = null)
    {
        return new JobDbModel(jobId ?? Guid.Empty, 
            userId ?? Guid.Empty, 
            jobStatus,
            startedTimeUtc ?? new DateTime(2023, 9, 1));
    }
}