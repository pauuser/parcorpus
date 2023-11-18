using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Search result (concordance) DTO model
/// </summary>
public class ConcordanceDto
{
    /// <summary>
    /// Source word
    /// </summary>
    /// <example>яблоко</example>
    [JsonPropertyName("source_word")]
    public string? SourceWord { get; set; }

    /// <summary>
    /// Aligned word
    /// </summary>
    /// <example>apple</example>
    [JsonPropertyName("aligned_word")]
    public string? AlignedWord { get; set; }

    /// <summary>
    /// Source text
    /// </summary>
    /// <example>Я увидел яблоко</example>
    [JsonPropertyName("source_text")]
    public string? SourceText { get; set; }
    
    /// <summary>
    /// Aligned translation
    /// </summary>
    /// <example>I saw an apple</example>
    [JsonPropertyName("aligned_translation")]
    public string? AlignedTranslation { get; set; }
    
    /// <summary>
    /// Source text title
    /// </summary>
    /// <example>War and peace</example>
    [JsonPropertyName("title")]
    public string? Title { get; set; }

    /// <summary>
    /// Author of the text
    /// </summary>
    /// <example>L. N. Tolstoy</example>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    /// <summary>
    /// Source of the text
    /// </summary>
    /// <example>Russian State National Library</example>
    [JsonPropertyName("source")]
    public string? Source { get; set; }

    /// <summary>
    /// Year of the book creation
    /// </summary>
    /// <example>1886</example>
    [JsonPropertyName("creation_year")]
    public int? CreationYear { get; set; }

    /// <summary>
    /// When the text was added to the database
    /// </summary>
    /// <example>19.03.2023</example>
    [JsonPropertyName("add_date")]
    public DateTime AddDate { get; set; }

    /// <summary>
    /// Concordance DTO constructor
    /// </summary>
    /// <param name="sourceWord">source word</param>
    /// <param name="alignedWord"> aligned word</param>
    /// <param name="sourceText">source text</param>
    /// <param name="alignedTranslation">aligned translation</param>
    /// <param name="title">text title</param>
    /// <param name="author">author</param>
    /// <param name="source">source</param>
    /// <param name="creationYear">creation year</param>
    /// <param name="addDate">add date</param>
    public ConcordanceDto(string sourceWord, 
        string alignedWord, 
        string sourceText, 
        string alignedTranslation, 
        string title, 
        string author, 
        string source, 
        int creationYear, 
        DateTime addDate)
    {
        SourceWord = sourceWord;
        AlignedWord = alignedWord;
        SourceText = sourceText;
        AlignedTranslation = alignedTranslation;
        Title = title;
        Author = author;
        Source = source;
        CreationYear = creationYear;
        AddDate = addDate;
    }

    public ConcordanceDto()
    {
    }
}