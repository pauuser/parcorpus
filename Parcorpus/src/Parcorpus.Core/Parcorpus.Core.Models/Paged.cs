namespace Parcorpus.Core.Models;

public class Paged<T> : IEquatable<Paged<T>>
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
        var totalPages = pageSize == 0 ? 0 : totalCount / pageSize.Value;
        if (pageSize != 0 && totalCount % pageSize.Value != 0)
            totalPages++;
        
        PageNumber = pageNumber.Value;
        PageSize = pageSize.Value;
        TotalPages = totalPages;
        TotalCount = totalCount;
        Items = items;
    }

    public Paged()
    {
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Paged<T>? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return PageNumber == other.PageNumber && 
               PageSize == other.PageSize && 
               TotalPages == other.TotalPages && 
               TotalCount == other.TotalCount && 
               Items.SequenceEqual(other.Items);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(PageNumber, PageSize, TotalPages, TotalCount, Items);
    }
}