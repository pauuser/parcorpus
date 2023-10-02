using Parcorpus.API.Dto.Enums;

namespace Parcorpus.API.Dto;

/// <summary>
/// Job Dto
/// </summary>
public class JobDto
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid JobId { get; set; }

    /// <summary>
    /// Status of the job
    /// </summary>
    public JobStatusDto JobStatus { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="jobId">Id</param>
    /// <param name="jobStatus">Status of the job</param>
    public JobDto(Guid jobId, JobStatusDto jobStatus)
    {
        JobId = jobId;
        JobStatus = jobStatus;
    }
} 