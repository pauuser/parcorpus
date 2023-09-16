namespace Parcorpus.Core.Models;

public sealed class Concordance
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
}