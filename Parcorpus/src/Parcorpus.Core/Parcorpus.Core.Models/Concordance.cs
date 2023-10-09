namespace Parcorpus.Core.Models;

public sealed class Concordance : IEquatable<Concordance>
{
    public string SourceWord { get; set; }

    public string AlignedWord { get; set; }

    public string SourceText { get; set; }
    
    public string AlignedTranslation { get; set; }

    public string Title { get; set; }

    public string Author { get; set; }

    public string Source { get; set; }

    public int CreationYear { get; set; }

    public DateTime AddDate { get; set; }

    public Concordance(string sourceWord, 
        string alignedWord, 
        string sourceText, 
        string alignedTranslation, 
        string title, 
        string author, 
        string source, 
        int creationYear, 
        DateTime addDate)
    {
        SourceWord = sourceWord;
        AlignedWord = alignedWord;
        SourceText = sourceText;
        AlignedTranslation = alignedTranslation;
        Title = title;
        Author = author;
        Source = source;
        CreationYear = creationYear;
        AddDate = addDate;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(Concordance? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return SourceWord == other.SourceWord && 
               AlignedWord == other.AlignedWord && 
               SourceText == other.SourceText && 
               AlignedTranslation == other.AlignedTranslation && 
               Title == other.Title && 
               Author == other.Author && 
               Source == other.Source && 
               CreationYear == other.CreationYear && 
               AddDate.Equals(other.AddDate);
    }

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(SourceWord);
        hashCode.Add(AlignedWord);
        hashCode.Add(SourceText);
        hashCode.Add(AlignedTranslation);
        hashCode.Add(Title);
        hashCode.Add(Author);
        hashCode.Add(Source);
        hashCode.Add(CreationYear);
        hashCode.Add(AddDate);
        
        return hashCode.ToHashCode();
    }
}