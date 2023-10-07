using System.Text;

namespace Parcorpus.Core.Models;

public sealed class PaginationParameters
{
    public int? PageNumber { get; set; }

    public int? PageSize { get; set; }
    
    public bool Specified => PageNumber is not null && PageSize is not null;

    public bool OutOfRange(int totalCount)
    {
        var totalPages = totalCount / PageSize!.Value;
        if (totalCount % PageSize.Value != 0)
            totalPages++;

        return PageNumber < 1 || PageNumber > totalPages;
    }

    public PaginationParameters(int? pageNumber, int? pageSize)
    {
        if (pageNumber is null && pageSize is not null ||
            pageNumber is not null && pageSize is null)
            throw new ArgumentException("Page number and page size should be specified together as values or nulls");
        
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append($"PageNumber: {PageNumber}");
        sb.Append($"PageSize: {PageSize}");

        return sb.ToString();
    }
}