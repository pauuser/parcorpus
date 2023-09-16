using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Parcorpus.API.Dto;

/// <summary>
/// Model for text input (in form)
/// </summary>
public class InputTextDto
{
    /// <summary>
    /// Source language short name
    /// </summary>
    /// <remarks>Must be short! Examples: "ru" (Russian), "en" (English)</remarks>
    /// <example>ru</example>
    [JsonPropertyName("source_language_code")]
    public string SourceLanguageCode { get; set; }
    
    /// <summary>
    /// Target language code
    /// </summary>
    /// <remarks>Must be short! Examples: "ru" (Russian), "en" (English)</remarks>
    /// <example>en</example>
    [JsonPropertyName("target_language_code")]
    public string TargetLanguageCode { get; set; }
    
    /// <summary>
    /// Text title
    /// </summary>
    /// <example>War and Peace</example>>
    [JsonPropertyName("title")]
    public string Title { get; set; }
    
    /// <summary>
    /// Text author
    /// </summary>
    /// <example>L. N. Tolstoy</example>
    [JsonPropertyName("author")]
    public string Author { get; set; }
    
    /// <summary>
    /// Text source
    /// </summary>
    /// <example>Russian State National Library</example>
    [JsonPropertyName("source")]
    public string Source { get; set; }
    
    /// <summary>
    /// Text creation year
    /// </summary>
    /// <example>1886</example>
    [JsonPropertyName("creation_year")]
    public int CreationYear { get; set; }
    
    /// <summary>
    /// Text genres list
    /// </summary>
    [JsonPropertyName("genres")]
    public IEnumerable<string> Genres { get; set; }
    
    /// <summary>
    /// Source language text
    /// </summary>
    [FromForm(Name="SourceText")]
    public IFormFile SourceText { get; set; }
    
    /// <summary>
    /// Target language text
    /// </summary>
    [FromForm(Name="TargetText")]
    public IFormFile TargetText { get; set; }

    public InputTextDto()
    {
    }

    public InputTextDto(string sourceLanguageCode, 
        string targetLanguageCode, 
        string title, 
        string author, 
        string source, 
        int creationYear, 
        IEnumerable<string> genres, 
        IFormFile sourceText, 
        IFormFile targetText)
    {
        SourceLanguageCode = sourceLanguageCode;
        TargetLanguageCode = targetLanguageCode;
        Title = title;
        Author = author;
        Source = source;
        CreationYear = creationYear;
        Genres = genres;
        SourceText = sourceText;
        TargetText = targetText;
    }
}