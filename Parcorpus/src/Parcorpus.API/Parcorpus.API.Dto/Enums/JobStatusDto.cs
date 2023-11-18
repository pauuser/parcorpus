using System.Runtime.Serialization;

namespace Parcorpus.API.Dto.Enums;

/// <summary>
/// Job status Enum
/// </summary>
public enum JobStatusDto
{
    /// <summary>
    /// Text has just been uploaded
    /// </summary>
    [EnumMember(Value = "uploaded")]
    Uploaded, 
    
    /// <summary>
    /// Text is now aligning
    /// </summary>
    [EnumMember(Value = "aligning")]
    Aligning, 
    
    /// <summary>
    /// Text is being saved to database
    /// </summary>
    [EnumMember(Value = "saving")]
    Saving, 
    
    /// <summary>
    /// Text processing is finished
    /// </summary>
    [EnumMember(Value = "finished")]
    Finished, 
    
    /// <summary>
    /// Text processing has faield
    /// </summary>
    [EnumMember(Value = "failed")]
    Failed
}