using System.Text.Json.Serialization;

namespace Parcorpus.API.Dto;

/// <summary>
/// Wrapper for paged objects
/// </summary>
/// <typeparam name="T">Type of object to be paged</typeparam>
public sealed class PagedDto<T>
{
    /// <summary>
    /// Page information
    /// </summary>
    [JsonPropertyName("page_info")]
    public PageInfoDto PageInfo { get; set; }

    /// <summary>
    /// Paged items
    /// </summary>
    [JsonPropertyName("items")]
    public List<T> Items { get; set; }

    public PagedDto(PageInfoDto pageInfo, List<T> items)
    {
        PageInfo = pageInfo;
        Items = items;
    }

    public PagedDto()
    {
    }
}