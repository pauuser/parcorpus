namespace Parcorpus.Core.Models;

public sealed class Text : IEquatable<Text>
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

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Text? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other))
            return true;
        
        return TextId == other.TextId && 
               Title == other.Title && 
               Author == other.Author && 
               Source == other.Source && 
               CreationYear == other.CreationYear && 
               AddDate.Equals(other.AddDate) && 
               SourceLanguage.Equals(other.SourceLanguage) && 
               TargetLanguage.Equals(other.TargetLanguage) && 
               Genres.SequenceEqual(other.Genres) && 
               Sentences.SequenceEqual(other.Sentences) && 
               AddedBy.Equals(other.AddedBy);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(TextId);
        hashCode.Add(Title);
        hashCode.Add(Author);
        hashCode.Add(Source);
        hashCode.Add(CreationYear);
        hashCode.Add(AddDate);
        hashCode.Add(SourceLanguage);
        hashCode.Add(TargetLanguage);
        hashCode.Add(Genres);
        hashCode.Add(Sentences);
        hashCode.Add(AddedBy);
        
        return hashCode.ToHashCode();
    }
}