using System.Text;

namespace Parcorpus.Core.Models;

public sealed class Filter : IEquatable<Filter>
{
    public string? Genre { get; set; }

    public DateTime? StartDateTime { get; set; }
    
    public DateTime? EndDateTime { get; set; }
    
    public string? Author { get; set; }

    public Filter(string? genre, 
        DateTime? startDateTime, 
        DateTime? endDateTime, 
        string? author)
    {
        Genre = genre;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
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
        sb.Append($"Dates = [{StartDateTime}, {EndDateTime}]");
        
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
               Nullable.Equals(StartDateTime, other.StartDateTime) && 
               Nullable.Equals(EndDateTime, other.EndDateTime) && 
               Author == other.Author;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Genre, StartDateTime, EndDateTime, Author);
    }
}