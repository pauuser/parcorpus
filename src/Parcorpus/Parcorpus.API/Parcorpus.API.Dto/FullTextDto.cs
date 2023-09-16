using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Text model containing sentences
/// </summary>
public class FullTextDto
{
    /// <summary>
    /// New sentence aligned translation
    /// </summary>
    [JsonPropertyName("text")]
    public TextDto Text { get; set; }

    /// <summary>
    /// Sentences
    /// </summary>
    [JsonPropertyName("sentences")]
    public List<SentenceDto> Sentences { get; set; }

    public FullTextDto(TextDto text, List<SentenceDto> sentences)
    {
        Text = text;
        Sentences = sentences;
    }
}