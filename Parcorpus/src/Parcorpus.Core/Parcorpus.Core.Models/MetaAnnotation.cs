namespace Parcorpus.Core.Models;

public sealed class MetaAnnotation
{
    public string Title { get; set; }
    
    public string Author { get; set; }
    
    public string Source { get; set; }
    
    public int CreationYear { get; set; }
    
    public DateTime AddDate { get; set; }

    public MetaAnnotation(string title, 
        string author, 
        string source, 
        int creationYear, 
        DateTime addDate)
    {
        Title = title;
        Author = author;
        Source = source;
        CreationYear = creationYear;
        AddDate = addDate;
    }
}