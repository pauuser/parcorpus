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
        if (pageNumber is null)
            pageNumber = 1;
        if (pageSize is null)
            pageSize = totalCount;
        var totalPages = totalCount / pageSize.Value;
        
        PageNumber = pageNumber.Value;
        PageSize = pageSize.Value;
        TotalPages = totalPages == 0 ? 1 : totalPages;
        TotalCount = totalCount;
        Items = items;
    }
}