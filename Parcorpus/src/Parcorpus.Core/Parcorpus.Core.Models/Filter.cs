using System.Text;

namespace Parcorpus.Core.Models;

public sealed class Filter : IEquatable<Filter>
{
    public string? Genre { get; set; }

    public int? StartYear { get; set; }
    
    public int? EndYear { get; set; }
    
    public string? Author { get; set; }

    public Filter(string? genre, 
        int? startYear, 
        int? endYear, 
        string? author)
    {
        Genre = genre;
        StartYear = startYear;
        EndYear = endYear;
        Author = author;
    }

    public Filter()
    {
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"Genre = {Genre};");
        sb.Append($"Author = {Author};");
        sb.Append($"Dates = [{StartYear}, {EndYear}]");
        
        return sb.ToString();
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Filter? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return Genre == other.Genre && 
               Nullable.Equals(StartYear, other.EndYear) && 
               Nullable.Equals(StartYear, other.EndYear) && 
               Author == other.Author;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Genre, StartYear, EndYear, Author);
    }
}