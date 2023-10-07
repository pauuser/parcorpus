namespace Parcorpus.Core.Models;

public class PagedText
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
}