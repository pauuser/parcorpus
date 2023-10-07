using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Model to query concordance
/// </summary>
public class ConcordanceQueryDto
{
    /// <summary>
    /// Word to search
    /// </summary>
    [JsonPropertyName("word")]
    public string Word { get; set; }

    /// <summary>
    /// Searched word source language
    /// </summary>
    /// <remarks>Should be short: "en", "ru", "fr", etc</remarks>
    /// <example>"ru"</example>
    [JsonPropertyName("source_language_short_name")]
    public string SourceLanguageShortName { get; set; }
    
    /// <summary>
    /// Translation will be given in respect to the chosen language
    /// </summary>
    /// <remarks>Should be short: "en", "ru", "fr", etc</remarks>
    /// <example>"ru"</example>
    [JsonPropertyName("target_language_short_name")]
    public string TargetLanguageShortName { get; set; }

    /// <summary>
    /// Filter for the query
    /// </summary>
    [JsonPropertyName("filter")]
    public FilterDto? Filter { get; set; }
}