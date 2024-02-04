namespace Parcorpus.DB.Models;

public class FilterDbModel : IEquatable<FilterDbModel>
{
    public string? Genre { get; set; }

    public int? StartYear { get; set; }

    public int? EndYear { get; set; }

    public string? Author { get; set; }

    public FilterDbModel(string? genre, 
        int? startYear, 
        int? endYear, 
        string? author)
    {
        Genre = genre;
        StartYear = startYear;
        EndYear = endYear;
        Author = author;
    }

    public FilterDbModel()
    {
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(FilterDbModel? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return Genre == other.Genre && 
               Nullable.Equals(StartYear, other.StartYear) && 
               Nullable.Equals(EndYear, other.EndYear) && 
               Author == other.Author;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Genre, StartYear, EndYear, Author);
    }
}