using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
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
    [JsonPropertyName("job_id")]
    public Guid JobId { get; set; }

    /// <summary>
    /// Status of the job
    /// </summary>
    [JsonPropertyName("job_status")]
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