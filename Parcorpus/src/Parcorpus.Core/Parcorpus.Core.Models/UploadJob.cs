namespace Parcorpus.Core.Models;

public sealed class UploadJob
{
    public Guid UserId { get; set; }

    public BiText BiText { get; set; }

    public Guid JobId { get; set; }

    public UploadJob(Guid userId, BiText biText, Guid jobId)
    {
        UserId = userId;
        BiText = biText;
        JobId = jobId;
    }
}