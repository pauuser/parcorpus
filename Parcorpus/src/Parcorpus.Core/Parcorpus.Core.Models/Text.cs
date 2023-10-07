namespace Parcorpus.Core.Models;

public sealed class Text
{
    public int TextId { get; set; }
    
    public string Title { get; set; }

    public string Author { get; set; }

    public string Source { get; set; }

    public int CreationYear { get; set; }

    public DateTime AddDate { get; set; }

    public Language SourceLanguage { get; set; }

    public Language TargetLanguage { get; set; }

    public List<string> Genres { get; set; }

    public List<Sentence> Sentences { get; set; }

    public Guid AddedBy { get; set; }

    public Text(int textId, 
        string title, 
        string author, 
        string source, 
        int creationYear, 
        DateTime addDate, 
        Language sourceLanguage, 
        Language targetLanguage,
        List<string> genres,
        List<Sentence> sentences,
        Guid addedBy)
    {
        TextId = textId;
        Title = title;
        Author = author;
        Source = source;
        CreationYear = creationYear;
        AddDate = addDate;
        SourceLanguage = sourceLanguage;
        TargetLanguage = targetLanguage;
        Genres = genres;
        Sentences = sentences;
        AddedBy = addedBy;
    }
}