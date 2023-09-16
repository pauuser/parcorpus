using Parcorpus.Core.Models;
using Parcorpus.DataAccess.Models;

namespace Parcorpus.DataAccess.Converters;

public static class TextConverter
{
    public static Text ConvertDbModelToAppModel(TextDbModel text)
    {
        return new(textId: text.TextId,
            title: text.MetaAnnotationNavigation.Title,
            author: text.MetaAnnotationNavigation.Author,
            source: text.MetaAnnotationNavigation.Source,
            creationYear: text.MetaAnnotationNavigation.CreationYear,
            addDate: text.MetaAnnotationNavigation.AddDate,
            sourceLanguage: LanguageConverter.ConvertDbModelToAppModel(text.LanguagePairNavigation.FromLanguageNavigation),
            targetLanguage: LanguageConverter.ConvertDbModelToAppModel(text.LanguagePairNavigation.ToLanguageNavigation),
            genres: text.MetaAnnotationNavigation.MetaGenresNavigation.Select(mg => mg.GenreNavigation?.Name).ToList(),
            sentences: text.SentencesNavigation.Select(s => SentenceConverter
                .ConvertDbModelToAppModel(sentence: s, 
                    sourceLanguage: text.LanguagePairNavigation.FromLanguageNavigation, 
                    targetLanguage: text.LanguagePairNavigation.ToLanguageNavigation)).ToList());
    }
}