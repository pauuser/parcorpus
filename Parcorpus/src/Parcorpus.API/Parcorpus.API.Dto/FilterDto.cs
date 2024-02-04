using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Search filter model
/// </summary>
public class FilterDto
{
    /// <summary>
    /// Genre
    /// </summary>
    [JsonPropertyName("genre")]
    public string? Genre { get; set; }

    /// <summary>
    /// From year
    /// </summary>
    [JsonPropertyName("start_year")]
    public int? StartYear { get; set; }
    
    /// <summary>
    /// To year
    /// </summary>
    [JsonPropertyName("end_year")]
    public int? EndYear { get; set; }
    
    /// <summary>
    /// Author
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    public FilterDto(string? genre, int? startYear, int? endYear, string? author)
    {
        Genre = genre;
        StartYear = startYear;
        EndYear = endYear;
        Author = author;
    }

    public FilterDto()
    {
    }
}