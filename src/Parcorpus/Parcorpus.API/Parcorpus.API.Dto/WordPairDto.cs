using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Word pair DTO
/// </summary>
public class WordPairDto
{
    /// <summary>
    /// Source word
    /// </summary>
    /// <example>Je</example>
    [JsonPropertyName("source_word")]
    public string SourceWord { get; set; }

    /// <summary>
    /// Aligned translation
    /// </summary>
    /// <example>I</example>
    [JsonPropertyName("target_word")]
    public string TargetWord { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="sourceWord">source word</param>
    /// <param name="targetWord">target word</param>
    public WordPairDto(string sourceWord, string targetWord)
    {
        SourceWord = sourceWord;
        TargetWord = targetWord;
    }
}