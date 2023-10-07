using System.Text;

namespace Parcorpus.Core.Models;

public sealed class Filter
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
}