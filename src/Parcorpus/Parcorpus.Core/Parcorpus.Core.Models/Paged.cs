namespace Parcorpus.Core.Models;

public class Paged<T>
{
    public int PageNumber { get; set; }

    public int PageSize { get; set; }

    public int TotalPages { get; set; }

    public int TotalCount { get; set; }

    public List<T> Items { get; set; }

    public Paged(int? pageNumber, int? pageSize, int totalCount, List<T> items)
    {
        pageNumber ??= 1;
        pageSize ??= totalCount;
        var totalPages = totalCount / pageSize.Value;
        if (totalCount % pageSize.Value != 0)
            totalPages++;
        
        PageNumber = pageNumber.Value;
        PageSize = pageSize.Value;
        TotalPages = totalPages;
        TotalCount = totalCount;
        Items = items;
    }
}