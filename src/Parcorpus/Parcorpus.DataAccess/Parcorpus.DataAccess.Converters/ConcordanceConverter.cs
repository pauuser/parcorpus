using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class ConcordanceConverter
{
    public static Concordance ConvertWordToConcordance(WordDbModel word)
    {
        return new Concordance(sourceWord: word.SourceWord,
            alignedWord: word.AlignedWord,
            sourceText: word.SentenceNavigation.SourceText,
            alignedTranslation: word.SentenceNavigation.AlignedTranslation,
            title: word.SentenceNavigation.TextNavigation.MetaAnnotationNavigation.Title,
            author: word.SentenceNavigation.TextNavigation.MetaAnnotationNavigation.Author,
            source: word.SentenceNavigation.TextNavigation.MetaAnnotationNavigation.Source,
            creationYear: word.SentenceNavigation.TextNavigation.MetaAnnotationNavigation.CreationYear,
            addDate: word.SentenceNavigation.TextNavigation.MetaAnnotationNavigation.AddDate);
    }
}