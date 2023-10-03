using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Dto for paged objects
/// </summary>
public sealed class PageInfoDto
{
    /// <summary>
    /// Number of the page
    /// </summary>
    [JsonPropertyName("page_number")]
    public int PageNumber { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    [JsonPropertyName("page_size")]
    public int PageSize { get; set; }

    /// <summary>
    /// Total number of pages
    /// </summary>
    [JsonPropertyName("total_pages")]
    public int TotalPages { get; set; }

    /// <summary>
    /// Total count of objectss
    /// </summary>
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    public PageInfoDto(int pageNumber, int pageSize, int totalPages, int totalCount)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalPages = totalPages;
        TotalCount = totalCount;
    }
}