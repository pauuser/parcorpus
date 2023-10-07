namespace Parcorpus.DB.Models;

public class FilterDbModel
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
}