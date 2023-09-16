using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Search History DTO
/// </summary>
public sealed class SearchHistoryDto
{
    /// <summary>
    /// Id
    /// </summary>
    /// <example>1</example>
    [JsonPropertyName("search_history_id")]
    public int SearchHistoryId { get; set; }
    
    /// <summary>
    /// Word
    /// </summary>
    /// <example>manger</example>
    [JsonPropertyName("word")]
    public string Word { get; set; }

    /// <summary>
    /// Short name of the word's language
    /// </summary>
    /// <example>fr</example>
    [JsonPropertyName("source_language_short_name")]
    public string SourceLanguageShortName { get; set; }

    /// <summary>
    /// Language of translation
    /// </summary>
    /// <example>ru</example>
    [JsonPropertyName("destination_language_short_name")]
    public string DestinationLanguageShortName { get; set; }

    /// <summary>
    /// Filters
    /// </summary>
    [JsonPropertyName("filters")]
    public FilterDto Filters { get; set; }

    /// <summary>
    /// Search's timestamp
    /// </summary>
    [JsonPropertyName("query_timestamp_utc")]
    public DateTime QueryTimestampUtc { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="searchHistoryId">Id</param>
    /// <param name="word">Word</param>
    /// <param name="sourceLanguageShortName">Source language short name</param>
    /// <param name="destinationLanguageShortName">Translation language</param>
    /// <param name="filters">Filters</param>
    /// <param name="queryTimestampUtc">Timestamp</param>
    public SearchHistoryDto(int searchHistoryId, 
        string word, 
        string sourceLanguageShortName, 
        string destinationLanguageShortName, 
        FilterDto filters, 
        DateTime queryTimestampUtc)
    {
        SearchHistoryId = searchHistoryId;
        Word = word;
        SourceLanguageShortName = sourceLanguageShortName;
        DestinationLanguageShortName = destinationLanguageShortName;
        Filters = filters;
        QueryTimestampUtc = queryTimestampUtc;
    }
}