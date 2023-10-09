namespace Parcorpus.DB.Models;

public class FilterDbModel : IEquatable<FilterDbModel>
{
    public string? Genre { get; set; }

    public DateTime? StartDateTime { get; set; }

    public DateTime? EndDateTime { get; set; }

    public string? Author { get; set; }

    public FilterDbModel(string? genre, 
        DateTime? startDateTime, 
        DateTime? endDateTime, 
        string? author)
    {
        Genre = genre;
        StartDateTime = startDateTime;
        EndDateTime = endDateTime;
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
               Nullable.Equals(StartDateTime, other.StartDateTime) && 
               Nullable.Equals(EndDateTime, other.EndDateTime) && 
               Author == other.Author;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Genre, StartDateTime, EndDateTime, Author);
    }
}