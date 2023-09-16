namespace Parcorpus.Core.Models;

public sealed class ConcordanceQuery
{
    public Word SourceWord { get; set; }

    public Language DestinationLanguage { get; set; }

    public Filter Filters { get; set; }

    public ConcordanceQuery(Word sourceWord, Language destinationLanguage, Filter filters)
    {
        SourceWord = sourceWord;
        DestinationLanguage = destinationLanguage;
        Filters = filters;
    }
}