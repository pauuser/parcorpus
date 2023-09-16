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
    /// From date
    /// </summary>
    [JsonPropertyName("start_date_time")]
    public DateTime? StartDateTime { get; set; }
    
    /// <summary>
    /// To date
    /// </summary>
    [JsonPropertyName("end_date_time")]
    public DateTime? EndDateTime { get; set; }
    
    /// <summary>
    /// Author
    /// </summary>
    [JsonPropertyName("author")]
    public string? Author { get; set; }

    public FilterDto(string? genre, DateTime? startDateTime, DateTime? endDateTime, string? author)
    {
        Genre = genre;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
        Author = author;
    }
}