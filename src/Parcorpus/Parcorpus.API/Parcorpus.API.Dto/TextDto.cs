using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Model for returning text information
/// </summary>
public class TextDto
{
    /// <summary>
    /// Text id
    /// </summary>
    [JsonPropertyName("text_id")]
    public int? TextId { get; set; }
    
    /// <summary>
    /// Text title
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Text author
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// Text source
    /// </summary>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Creation year
    /// </summary>
    [JsonPropertyName("creation_year")]
    public int? CreationYear { get; set; }

    /// <summary>
    /// Add date
    /// </summary>
    [JsonPropertyName("add_date")]
    public DateTime? AddDate { get; set; }

    /// <summary>
    /// Source language short name
    /// </summary>
    [JsonPropertyName("source_language")]
    public string SourceLanguage { get; set; }

    /// <summary>
    /// Target language short name
    /// </summary>
    [JsonPropertyName("target_language")]
    public string TargetLanguage { get; set; }

    /// <summary>
    /// User Id of the user who uploaded the text
    /// </summary>
    [JsonPropertyName("added_by")]
    public Guid AddedBy { get; set; }

    /// <summary>
    /// Text DTO constructor
    /// </summary>
    /// <param name="textId">text id</param>
    /// <param name="title">title</param>
    /// <param name="author">author</param>
    /// <param name="source">source</param>
    /// <param name="creationYear">creation year</param>
    /// <param name="addDate">add date</param>
    /// <param name="sourceLanguage">source language</param>
    /// <param name="targetLanguage">target language</param>
    /// <param name="addedBy">user id</param>
    public TextDto(int? textId, 
        string? title, 
        string? author, 
        string? source, 
        int? creationYear, 
        DateTime? addDate, 
        string sourceLanguage, 
        string targetLanguage,
        Guid addedBy)
    {
        TextId = textId;
        Title = title;
        Author = author;
        Source = source;
        CreationYear = creationYear;
        AddDate = addDate;
        SourceLanguage = sourceLanguage;
        TargetLanguage = targetLanguage;
        AddedBy = addedBy;
    }
}