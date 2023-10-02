namespace Parcorpus.API.Dto.Enums;

/// <summary>
/// Job status Enum
/// </summary>
public enum JobStatusDto
{
    /// <summary>
    /// Text has just been uploaded
    /// </summary>
    Uploaded, 
    
    /// <summary>
    /// Text is now aligning
    /// </summary>
    Aligning, 
    
    /// <summary>
    /// Text is being saved to database
    /// </summary>
    Saving, 
    
    /// <summary>
    /// Text processing is finished
    /// </summary>
    Finished, 
    
    /// <summary>
    /// Text processing has faield
    /// </summary>
    Failed
}