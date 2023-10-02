namespace Parcorpus.Core.Models;

public sealed class PaginationParameters
{
    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }
    
    public bool Specified => PageNumber is not null && PageSize is not null;

    public PaginationParameters(int? pageNumber, int? pageSize)
    {
        if (pageNumber is null && pageSize is not null ||
            pageNumber is not null && pageSize is null)
            throw new ArgumentException("Page number and page size should be specified together as values or nulls");
        
        PageNumber = pageNumber;
        PageSize = pageSize;
    }
}