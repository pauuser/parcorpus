namespace Parcorpus.Core.Models;

public class PagedText : IEquatable<PagedText>
{
    public int SentencesPageNumber { get; set; }

    public int SentencesPageSize { get; set; }

    public int SentencesTotalPages { get; set; }

    public int SentencesTotalCount { get; set; }

    public Text Text { get; set; }

    public PagedText(int? sentencesPageNumber, int? sentencesPageSize, int sentencesTotalCount, Text text)
    {
        sentencesPageNumber ??= 1;
        sentencesPageSize ??= sentencesTotalCount;
        var totalPages = sentencesTotalCount / sentencesPageSize.Value;
        
        SentencesPageNumber = sentencesPageNumber.Value;
        SentencesPageSize = sentencesPageSize.Value;
        SentencesTotalPages = totalPages == 0 ? 1 : totalPages;
        SentencesTotalCount = sentencesTotalCount;
        Text = text;
    }

    public override bool Equals(object? obj)
    {
        return base.Equals(obj);
    }

    public bool Equals(PagedText? other)
    {
        if (ReferenceEquals(null, other)) 
            return false;
        if (ReferenceEquals(this, other)) 
            return true;
        
        return SentencesPageNumber == other.SentencesPageNumber && 
               SentencesPageSize == other.SentencesPageSize && 
               SentencesTotalPages == other.SentencesTotalPages && 
               SentencesTotalCount == other.SentencesTotalCount && 
               Text.Equals(other.Text);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SentencesPageNumber, SentencesPageSize, SentencesTotalPages, SentencesTotalCount, Text);
    }
}