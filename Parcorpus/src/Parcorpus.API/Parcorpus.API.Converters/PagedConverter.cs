using Parcorpus.API.Dto;
using Parcorpus.Core.Models;

namespace Parcorpus.API.Converters;

public static class PagedConverter
{
    public static PagedDto<ConcordanceDto> ConvertAppModelToDto(Paged<Concordance> obj)
    {
        return new PagedDto<ConcordanceDto>(pageInfo: ConvertPaging(obj),
            items: obj.Items.Select(ConcordanceConverter.ConvertAppModelToDto).ToList());
    }

    private static PageInfoDto ConvertPaging<T>(Paged<T> obj)
    {
        return new PageInfoDto(pageNumber: obj.PageNumber,
            pageSize: obj.PageSize,
            totalPages: obj.TotalPages,
            totalCount: obj.TotalCount);
    }
    
    public static PagedDto<TextDto> ConvertAppModelToDto(Paged<Text> obj)
    {
        return new PagedDto<TextDto>(pageInfo: ConvertPaging(obj),
            items: obj.Items.Select(TextConverter.ConvertAppModelToDto).ToList());
    }
    
    public static PagedDto<SearchHistoryDto> ConvertAppModelToDto(Paged<SearchHistoryRecord> obj)
    {
        return new PagedDto<SearchHistoryDto>(pageInfo: ConvertPaging(obj),
            items: obj.Items.Select(SearchHistoryConverter.ConvertAppModelToDto).ToList());
    }
    
    public static PagedDto<JobDto> ConvertAppModelToDto(Paged<ProgressJob> obj)
    {
        return new PagedDto<JobDto>(pageInfo: ConvertPaging(obj),
            items: obj.Items.Select(JobConverter.ConvertAppModelToDto).ToList());
    }

    public static PagedTextDto ConvertAppModelToDto(PagedText obj)
    {
        return new PagedTextDto(pageInfo: ConvertPaging(obj),
            text: TextConverter.ConvertFullTextToDto(obj.Text));
    }
    
    private static PageInfoDto ConvertPaging(PagedText obj)
    {
        return new PageInfoDto(pageNumber: obj.SentencesPageNumber,
            pageSize: obj.SentencesPageSize,
            totalPages: obj.SentencesTotalPages,
            totalCount: obj.SentencesTotalCount);
    }
}