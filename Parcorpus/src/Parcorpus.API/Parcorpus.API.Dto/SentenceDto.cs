using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Dto to edit sentence
/// </summary>
public class SentenceDto
{
    /// <summary>
    /// Sentence id
    /// </summary>
    /// <example>1</example>>
    [JsonPropertyName("sentence_id")]
    public int? SentenceId { get; set; }
    
    /// <summary>
    /// New sentence source text
    /// </summary>
    /// <example>I saw an apple</example>
    [JsonPropertyName("source_text")]
    public string? SourceText { get; set; }

    /// <summary>
    /// New sentence aligned translation
    /// </summary>
    /// <example>Я увидел яблоко</example>
    [JsonPropertyName("aligned_translation")]
    public string? AlignedTranslation { get; set; }

    /// <summary>
    /// Aligned words in sentence
    /// </summary>
    [JsonPropertyName("words")]
    public List<WordPairDto> Words { get; set; }

    /// <summary>
    /// Sentence dto constructor
    /// </summary>
    /// <param name="sentenceId">Sentence id</param>
    /// <param name="sourceText">New source text</param>
    /// <param name="alignedTranslation">New aligned translation</param>
    /// <param name="words">Aligned words</param>
    public SentenceDto(int? sentenceId, string? sourceText, string? alignedTranslation, List<WordPairDto> words)
    {
        SentenceId = sentenceId;
        SourceText = sourceText;
        AlignedTranslation = alignedTranslation;
        Words = words;
    }
}