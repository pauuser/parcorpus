using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Dto for text paged by sentences
/// </summary>
public class PagedTextDto
{
    /// <summary>
    /// Page information
    /// </summary>
    [JsonPropertyName("page_info")]
    public PageInfoDto PageInfo { get; set; }

    /// <summary>
    /// Full text with paged sentences
    /// </summary>
    [JsonPropertyName("text")]
    public FullTextDto Text { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pageInfo"></param>
    /// <param name="text"></param>
    public PagedTextDto(PageInfoDto pageInfo, FullTextDto text)
    {
        PageInfo = pageInfo;
        Text = text;
    }
}